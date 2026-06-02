using System;
using PasserCard.Boss;
using PasserCard.Cards;
using PasserCard.Encounter;
using PasserCard.Map;
using PasserCard.Run;
using PasserCard.UI;
using UnityEngine;

namespace PasserCard.Core
{
    /// <summary>
    /// Full roguelike run entry: map navigation, encounters, charon tribute, boss fights.
    /// </summary>
    public sealed class RunBootstrap : MonoBehaviour
    {
        [SerializeField] private RunIdentityId identityId = RunIdentityId.Ferryman;
        [SerializeField] private StartingDeckId deckId = StartingDeckId.Standard;
        [SerializeField] private int act = 1;
        [SerializeField] private BossId actBoss = BossId.Nyx;
        [SerializeField] private bool startRunOnPlay = true;
        [SerializeField] private bool useMapUI = true;
        [SerializeField] private bool useEncounterUI = true;

        private RunState? _run;

        public RunState? Run => _run;

        private void Start()
        {
            if (startRunOnPlay)
            {
                StartNewRun();
            }
        }

        [ContextMenu("Start New Run")]
        public void StartNewRun()
        {
            var identity = RunIdentityLibrary.Get(identityId);
            var deckDef = StartingDeckLibrary.Get(deckId);
            var random = new System.Random();
            var wallet = new SoulCoinWallet(identity.StartingCoins);
            var deck = deckDef.BuildDeck(random);
            var map = MapGenerator.GenerateAct(act, random, actBoss);

            _run = new RunState(identity, deckDef, map, wallet, deck, random);
            _run.Map.MarkVisited(_run.Map.StartNodeId);

            if (useMapUI)
            {
                var mapUi = GetComponent<RunMapUIController>();
                if (mapUi == null)
                {
                    mapUi = gameObject.AddComponent<RunMapUIController>();
                }

                mapUi.Bind(this);
            }

            Debug.Log($"[PasserCard] Run started. Identity={identity.DisplayName}, Deck={deckDef.DisplayName}, Act={act}");
        }

        public void BeginEncounterForNode(int nodeId)
        {
            if (_run == null)
            {
                return;
            }

            var node = _run.Map.GetNode(nodeId);
            if (node == null || !node.IsAvailable)
            {
                return;
            }

            _run.CurrentNodeId = nodeId;
            _run.Map.MarkVisited(nodeId);

            switch (node.Type)
            {
                case MapNodeType.Normal:
                case MapNodeType.Elite:
                case MapNodeType.Boss:
                    StartCombatEncounter(node);
                    break;
                case MapNodeType.Charon:
                    HandleCharonNode();
                    break;
                case MapNodeType.Shop:
                    HandleShopNode();
                    break;
                case MapNodeType.Planet:
                    HandlePlanetNode();
                    break;
            }

            RefreshMapUi();
        }

        private void StartCombatEncounter(MapNode node)
        {
            if (_run == null)
            {
                return;
            }

            var config = _run.BuildEncounterConfig(node);
            _run.ActiveEncounter = new EncounterSession(config, _run.RunDeck, _run.Wallet, _run.Random);
            _run.ActiveEncounter.StartEncounter();
            _run.Phase = RunPhase.Encounter;

            if (useEncounterUI)
            {
                var ui = GetComponent<EncounterUIController>();
                if (ui == null)
                {
                    ui = gameObject.AddComponent<EncounterUIController>();
                }

                ui.Bind(_run.ActiveEncounter, OnEncounterFinished);
            }
        }

        private void OnEncounterFinished(EncounterSession session)
        {
            if (_run == null)
            {
                return;
            }

            _run.SyncDeckFromEncounter();
            _run.ActiveEncounter = null;
            _run.Phase = RunPhase.Map;

            if (session.IsVictory && _run.CurrentNode?.Type == MapNodeType.Boss)
            {
                _run.Phase = RunPhase.Complete;
                _run.LastEventMessage = "Act Boss 击败！可进入下一 Act。";
            }
            else if (!session.IsVictory)
            {
                _run.LastEventMessage = "战斗失败，魂币受损。";
            }
            else
            {
                _run.LastEventMessage = $"战斗胜利。魂币 {_run.Wallet.Balance}";
            }

            RefreshMapUi();
        }

        private void HandleCharonNode()
        {
            if (_run == null)
            {
                return;
            }

            if (CharonTributeSystem.TryPayTribute(_run.Wallet, _run.Map.Act, _run.Identity.CharonDiscount, out var paid))
            {
                _run.LastEventMessage = $"已向卡戎献祭 {paid} 魂币，可继续旅程。";
            }
            else
            {
                _run.Phase = RunPhase.Failed;
                _run.LastEventMessage = "魂币不足，无法献祭卡戎。旅程终止。";
            }

            _run.Phase = _run.Phase == RunPhase.Failed ? RunPhase.Failed : RunPhase.Map;
        }

        private void HandleShopNode()
        {
            if (_run == null)
            {
                return;
            }

            var cost = (int)Math.Round(6 * _run.Identity.ShopPriceMultiplier);
            if (_run.Wallet.TrySpend(cost))
            {
                _run.RunDeck.Add(PlayingCardFactory.Create(Suit.Hearts, Rank.Jack, Rank.Four));
                _run.LastEventMessage = $"商店：花费 {cost} 魂币购入一张红心 J/4。";
            }
            else
            {
                _run.LastEventMessage = "魂币不足，离开商店。";
            }
        }

        private void HandlePlanetNode()
        {
            if (_run == null)
            {
                return;
            }

            if (_run.Random.NextDouble() < 0.5d)
            {
                _run.Wallet.Add(4);
                _run.LastEventMessage = "星球事件：冥河赠礼 +4 魂币。";
            }
            else
            {
                _run.Wallet.TrySpend(2);
                _run.LastEventMessage = "星球事件：迷雾代价 −2 魂币。";
            }
        }

        private void RefreshMapUi()
        {
            var mapUi = GetComponent<RunMapUIController>();
            mapUi?.Refresh();
        }
    }
}
