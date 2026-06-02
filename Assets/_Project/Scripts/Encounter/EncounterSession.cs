using System;
using System.Collections.Generic;
using PasserCard.Cards;
using PasserCard.Enchantments;
using PasserCard.Gameplay;
using PasserCard.Pass;
using PasserCard.Poker;
using PasserCard.Run;
using PasserCard.Table;

namespace PasserCard.Encounter
{
    public sealed class EncounterSession
    {
        private readonly EncounterConfig _config;
        private readonly Deck _drawPile;
        private readonly Hand _hand;
        private readonly PlayArea _playArea;
        private readonly PassState _passState = new();
        private readonly Random _random;
        private readonly List<PlayingCardInstance> _burnPile = new();
        private bool _passedThisQuota;

        public EncounterSession(
            EncounterConfig config,
            IEnumerable<PlayingCardInstance> runDeck,
            SoulCoinWallet wallet,
            Random? random = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            Wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
            _random = random ?? new Random();

            _drawPile = new Deck();
            _drawPile.AddRange(runDeck);
            _drawPile.Shuffle();

            _hand = new Hand(config.HandSize);
            _playArea = new PlayArea();

            Phase = EncounterPhase.QuotaSetup;
            CurrentQuota = 0;
            DiscardsRemaining = config.DiscardsRemaining;
            BestScore = 0;
            LastQuotaScore = 0;
        }

        public EncounterPhase Phase { get; private set; }
        public int CurrentQuota { get; private set; }
        public int DiscardsRemaining { get; private set; }
        public int BestScore { get; private set; }
        public int LastQuotaScore { get; private set; }
        public bool IsVictory => BestScore >= _config.TargetScore;
        public bool IsComplete => Phase == EncounterPhase.EncounterComplete;

        public SoulCoinWallet Wallet { get; }
        public Hand Hand => _hand;
        public PlayArea PlayArea => _playArea;
        public PassState PassState => _passState;
        public EncounterConfig Config => _config;
        public int PassesUsed => _passState.PassesUsed;
        public int PassLimit => _config.PassLimit + _config.IdentityExtraPasses;
        public int PassesRemaining => Math.Max(0, PassLimit - _passState.PassesUsed);
        public int TargetScore => _config.TargetScore;
        public IReadOnlyList<PlayingCardInstance> BurnPile => _burnPile;

        public string PhaseLabel => Phase switch
        {
            EncounterPhase.Discard => "弃牌",
            EncounterPhase.PrePinFog => "预钉迷雾",
            EncounterPhase.Pass => "Pass",
            EncounterPhase.PlayToSlots => "出牌",
            EncounterPhase.ResolveQuota => "结算",
            EncounterPhase.EncounterComplete => "结束",
            _ => "准备"
        };

        public void StartEncounter()
        {
            FillHand();
            BeginQuota();
        }

        public void BeginQuota()
        {
            if (CurrentQuota >= _config.PlayQuotas)
            {
                CompleteEncounter();
                return;
            }

            CurrentQuota++;
            Phase = EncounterPhase.QuotaSetup;
            _passState.PassesUsed = 0;
            _passState.MustDoublePassNext = false;
            _passedThisQuota = false;

            for (var i = 0; i < _hand.Count; i++)
            {
                _hand.Cards[i].ResetQuotaFlags();
            }

            _playArea.RefreshFog(_config.MistFogSlots, _random);
            Phase = EncounterPhase.Discard;
        }

        public bool TryDiscard(Guid instanceId)
        {
            if (Phase != EncounterPhase.Discard && Phase != EncounterPhase.PrePinFog && Phase != EncounterPhase.Pass)
            {
                return false;
            }

            if (DiscardsRemaining <= 0)
            {
                return false;
            }

            if (!_hand.TryRemove(instanceId, out var card))
            {
                return false;
            }

            _drawPile.Add(card);
            _drawPile.Shuffle();
            if (!_drawPile.TryDraw(out var replacement))
            {
                return false;
            }

            _hand.TryAdd(replacement);
            DiscardsRemaining--;
            return true;
        }

        public void EnterPrePinPhase() => Phase = EncounterPhase.PrePinFog;

        public void EnterPassPhase() => Phase = EncounterPhase.Pass;

        public bool TryPinToFogSlot(int slotIndex, Guid cardInstanceId)
        {
            if (Phase != EncounterPhase.PrePinFog && Phase != EncounterPhase.Discard)
            {
                return false;
            }

            if (!_hand.TryRemove(cardInstanceId, out var card))
            {
                return false;
            }

            if (!_playArea.TryPinToFogSlot(slotIndex, card))
            {
                _hand.TryAdd(card);
                return false;
            }

            return true;
        }

        public PassResult TryPassCard(Guid instanceId)
        {
            if (Phase != EncounterPhase.Pass)
            {
                return new PassResult(false, "Not in pass phase.", false);
            }

            for (var i = 0; i < _hand.Count; i++)
            {
                var card = _hand.Cards[i];
                if (card.InstanceId != instanceId)
                {
                    continue;
                }

                var result = SuitPassRules.TryPass(card, _passState, Wallet, PassLimit, _config, _random);
                if (result.Success)
                {
                    _passedThisQuota = true;
                    EnchantmentRules.OnPass(card, _hand.Cards, i);
                    if (EnchantmentRules.TrySplitWarCrack(card, _random, out var half) && half != null)
                    {
                        _drawPile.Add(half);
                        _drawPile.Shuffle();
                    }
                }

                return result;
            }

            return new PassResult(false, "Card not in hand.", false);
        }

        public void EnterPlayPhase() => Phase = EncounterPhase.PlayToSlots;

        public bool TryPlaceCard(int slotIndex, Guid cardInstanceId)
        {
            if (Phase != EncounterPhase.PlayToSlots)
            {
                return false;
            }

            if (!_hand.TryRemove(cardInstanceId, out var card))
            {
                return false;
            }

            if (!_playArea.TryPlaceCard(slotIndex, card))
            {
                _hand.TryAdd(card);
                return false;
            }

            return true;
        }

        public PokerHandResult ResolveCurrentQuota(float extraMult = 1f)
        {
            if (!TryResolveCurrentQuota(out var result, out var error, extraMult))
            {
                throw new InvalidOperationException(error);
            }

            return result;
        }

        public bool TryResolveCurrentQuota(out PokerHandResult result, out string error, float extraMult = 1f)
        {
            if (Phase != EncounterPhase.PlayToSlots)
            {
                result = default;
                error = "Not in play phase.";
                return false;
            }

            var scoringCards = new List<ScoringCard>();
            var committed = _playArea.GetCommittedCards();
            for (var i = 0; i < _playArea.Slots.Count; i++)
            {
                var slot = _playArea.Slots[i];
                if (slot.Card == null)
                {
                    continue;
                }

                var riftMult = TableEnvironmentRules.RiftCrackSplitBonus(_config, slot.Card);
                scoringCards.Add(SuitPassRules.ToScoringCard(slot.Card, slot.IsFog, riftMult));
            }

            if (scoringCards.Count == 0)
            {
                result = default;
                error = "At least one card must be committed.";
                return false;
            }

            ApplyThornUnpassedCosts(committed);

            var mult = extraMult
                       * SuitPassRules.GetHeartChipsPenalty(_passState)
                       * _config.IdentityScoreMultiplier
                       * EnchantmentRules.GetScoreMultModifier(committed, CurrentQuota, _config.PlayQuotas);

            result = PokerHandEvaluator.Evaluate(scoringCards, mult);
            LastQuotaScore = result.TotalScore;
            if (result.TotalScore > BestScore)
            {
                BestScore = result.TotalScore;
            }

            Phase = EncounterPhase.ResolveQuota;
            error = string.Empty;
            return true;
        }

        public void FinishQuota()
        {
            if (Phase != EncounterPhase.ResolveQuota)
            {
                return;
            }

            if (!IsVictory && CurrentQuota < _config.PlayQuotas)
            {
                var loss = _config.RetaliationCoinLoss;
                if (!_passedThisQuota && _config.BossId == Boss.BossId.Cerberus)
                {
                    loss += 1;
                }

                Wallet.TrySpend(loss);
            }

            ReturnCommittedCardsToDeck();
            FillHand();
            BeginQuota();
        }

        public void CompleteEncounter()
        {
            Phase = EncounterPhase.EncounterComplete;
            if (IsVictory)
            {
                var reward = (int)Math.Round(_config.VictoryCoinReward * _config.IdentityCoinGainMultiplier);
                Wallet.Add(reward);
            }
            else
            {
                Wallet.TrySpend(_config.DefeatCoinLoss);
            }
        }

        public List<PlayingCardInstance> CollectRunDeckSnapshot()
        {
            var cards = new List<PlayingCardInstance>();
            cards.AddRange(_drawPile.Cards);
            cards.AddRange(_hand.Cards);
            for (var i = 0; i < _playArea.Slots.Count; i++)
            {
                if (_playArea.Slots[i].Card != null)
                {
                    cards.Add(_playArea.Slots[i].Card!);
                }
            }

            return cards;
        }

        private void ApplyThornUnpassedCosts(IReadOnlyList<PlayingCardInstance> committed)
        {
            for (var i = 0; i < committed.Count; i++)
            {
                var cost = TableEnvironmentRules.ThornUnpassedPlayCost(committed[i], _config);
                if (cost > 0)
                {
                    Wallet.TrySpend(cost);
                }
            }
        }

        private void FillHand()
        {
            while (_hand.Count < _config.HandSize && _drawPile.TryDraw(out var card))
            {
                _hand.TryAdd(card);
            }
        }

        private void ReturnCommittedCardsToDeck()
        {
            for (var i = 0; i < _playArea.Slots.Count; i++)
            {
                var card = _playArea.Slots[i].ClearCard();
                if (card == null)
                {
                    continue;
                }

                card.ResetQuotaFlags();
                if (EnchantmentRules.ShouldBurnAfterPlay(card))
                {
                    _burnPile.Add(card);
                    continue;
                }

                _drawPile.Add(card);
            }

            _drawPile.Shuffle();
        }
    }
}
