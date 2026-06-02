using PasserCard.Cards;
using PasserCard.Enchantments;
using PasserCard.Poker;
using PasserCard.Run;
using PasserCard.Table;

namespace PasserCard.Pass
{
    public sealed class PassState
    {
        public int PassesUsed { get; set; }
        public int HeartPassCoinLayers { get; set; }
        public bool MustDoublePassNext { get; set; }
    }

    public readonly struct PassResult
    {
        public PassResult(bool success, string message, bool consumedPassToken)
        {
            Success = success;
            Message = message;
            ConsumedPassToken = consumedPassToken;
        }

        public bool Success { get; }
        public string Message { get; }
        public bool ConsumedPassToken { get; }
    }

    public static class SuitPassRules
    {
        public static PassResult TryPass(
            PlayingCardInstance card,
            PassState state,
            SoulCoinWallet wallet,
            int passLimit,
            Encounter.EncounterConfig? config = null,
            System.Random? random = null)
        {
            if (card.IsPinnedToFogSlot)
            {
                return new PassResult(false, "Pinned fog cards cannot pass.", false);
            }

            if (!EnchantmentRules.CanPass(card))
            {
                return new PassResult(false, "Fold-locked card cannot pass.", false);
            }

            if (config != null && !TableEnvironmentRules.TryPassThornTable(card, config, wallet))
            {
                return new PassResult(false, "Not enough soul coins for thorn table pass.", false);
            }

            if (MustDoublePass(card, state))
            {
                card.TogglePassFace();
                card.TogglePassFace();
                state.MustDoublePassNext = false;
                card.WasPassedThisQuota = true;
                return new PassResult(true, "Double pass resolved.", false);
            }

            var consumesToken = card.Suit != Suit.Clubs || card.IsPlagued;
            if (consumesToken && state.PassesUsed >= passLimit)
            {
                return new PassResult(false, "Pass limit reached.", false);
            }

            if (!TryPayPassCost(card, wallet, out var payMessage))
            {
                return new PassResult(false, payMessage, false);
            }

            card.TogglePassFace();
            card.WasPassedThisQuota = true;

            if (consumesToken)
            {
                state.PassesUsed++;
            }

            ApplyPostPassEffects(card, state, wallet);

            var result = new PassResult(true, "Passed.", consumesToken);
            if (config != null && random != null)
            {
                result = TableEnvironmentRules.ApplyPostPassTableEffect(card, result, config, state, random);
            }

            return result;
        }

        public static ScoringCard ToScoringCard(PlayingCardInstance card, bool inFogSlot, float riftMult = 1f)
        {
            var rank = card.EffectiveRank;
            var countsForStraight = true;
            var countsForFlush = true;
            var bonusChips = 0;

            if (!inFogSlot && !card.PassEffectsSuppressed)
            {
                ApplyScoringModifiers(card, ref rank, ref countsForStraight, ref countsForFlush, ref bonusChips);
            }

            if (riftMult > 1f)
            {
                bonusChips += (int)System.Math.Round(10f * (riftMult - 1f));
            }

            return new ScoringCard(rank, card.Suit, countsForStraight, countsForFlush, bonusChips);
        }

        private static bool MustDoublePass(PlayingCardInstance card, PassState state)
        {
            return state.MustDoublePassNext && card.Suit == Suit.Clubs && !card.IsPlagued;
        }

        private static bool TryPayPassCost(PlayingCardInstance card, SoulCoinWallet wallet, out string message)
        {
            if (card.IsPlagued && card.PlagueSuit == PlagueSuit.RustRot)
            {
                if (!wallet.TrySpend(1))
                {
                    message = "Not enough soul coins for rust rot pass.";
                    return false;
                }
            }
            else if (card.Suit == Suit.Diamonds)
            {
                if (!wallet.TrySpend(1))
                {
                    message = "Not enough soul coins for diamond pass.";
                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        private static void ApplyPostPassEffects(PlayingCardInstance card, PassState state, SoulCoinWallet wallet)
        {
            if (card.IsPlagued)
            {
                ApplyPlaguePostPass(card, state, wallet);
                return;
            }

            switch (card.Suit)
            {
                case Suit.Hearts:
                    wallet.Add(1);
                    state.HeartPassCoinLayers = System.Math.Min(3, state.HeartPassCoinLayers + 1);
                    break;
                case Suit.Clubs:
                    state.MustDoublePassNext = true;
                    break;
            }
        }

        private static void ApplyPlaguePostPass(PlayingCardInstance card, PassState state, SoulCoinWallet wallet)
        {
            switch (card.PlagueSuit)
            {
                case PlagueSuit.FesterBlood:
                    wallet.Add(1);
                    wallet.TrySpend(1);
                    break;
                case PlagueSuit.WitherBlight:
                    state.MustDoublePassNext = true;
                    break;
            }
        }

        private static void ApplyScoringModifiers(
            PlayingCardInstance card,
            ref Rank rank,
            ref bool countsForStraight,
            ref bool countsForFlush,
            ref int bonusChips)
        {
            if (card.IsPlagued)
            {
                ApplyPlagueScoring(card, ref rank, ref countsForStraight, ref countsForFlush, ref bonusChips);
                return;
            }

            switch (card.Suit)
            {
                case Suit.Spades:
                    rank = RankExtensions.ClampRank(rank.ToSortValue() + 1);
                    countsForStraight = false;
                    break;
                case Suit.Diamonds:
                    bonusChips += card.IsPlagued ? 7 : 15;
                    break;
            }
        }

        private static void ApplyPlagueScoring(
            PlayingCardInstance card,
            ref Rank rank,
            ref bool countsForStraight,
            ref bool countsForFlush,
            ref int bonusChips)
        {
            switch (card.PlagueSuit)
            {
                case PlagueSuit.CorruptSpring:
                    rank = RankExtensions.ClampRank(rank.ToSortValue() + 1);
                    countsForStraight = false;
                    countsForFlush = false;
                    break;
                case PlagueSuit.RustRot:
                    bonusChips += 7;
                    break;
            }
        }

        public static float GetHeartChipsPenalty(PassState state)
        {
            return 1f - 0.1f * state.HeartPassCoinLayers;
        }
    }
}
