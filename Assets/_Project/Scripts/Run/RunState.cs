using System;
using System.Collections.Generic;
using PasserCard.Boss;
using PasserCard.Cards;
using PasserCard.Encounter;
using PasserCard.Map;

namespace PasserCard.Run
{
    public enum RunPhase
    {
        Map = 0,
        Encounter = 1,
        Event = 2,
        Complete = 3,
        Failed = 4
    }

    public sealed class RunState
    {
        public RunState(
            RunIdentityDefinition identity,
            StartingDeckDefinition deckDefinition,
            ActMap map,
            SoulCoinWallet wallet,
            List<PlayingCardInstance> runDeck,
            Random random)
        {
            Identity = identity;
            DeckDefinition = deckDefinition;
            Map = map;
            Wallet = wallet;
            RunDeck = runDeck;
            Random = random;
            Phase = RunPhase.Map;
            CurrentNodeId = map.StartNodeId;
        }

        public RunIdentityDefinition Identity { get; }
        public StartingDeckDefinition DeckDefinition { get; }
        public ActMap Map { get; }
        public SoulCoinWallet Wallet { get; }
        public List<PlayingCardInstance> RunDeck { get; }
        public Random Random { get; }
        public RunPhase Phase { get; set; }
        public int CurrentNodeId { get; set; }
        public EncounterSession? ActiveEncounter { get; set; }
        public string LastEventMessage { get; set; } = string.Empty;

        public MapNode? CurrentNode => Map.GetNode(CurrentNodeId);

        public EncounterConfig BuildEncounterConfig(MapNode node)
        {
            if (node.Type == MapNodeType.Boss)
            {
                var boss = BossLibrary.Get(node.BossId);
                var config = boss.ToEncounterConfig();
                Identity.ApplyToConfig(config);
                return config;
            }

            var normal = new EncounterConfig
            {
                TargetScore = node.Type == MapNodeType.Elite ? 150 : 120,
                MistFogSlots = node.Type == MapNodeType.Elite ? 3 : 2,
                TableEnvironment = Map.Act switch
                {
                    1 => Table.TableEnvironmentId.FogMist,
                    2 => Table.TableEnvironmentId.SmoothIce,
                    3 => Table.TableEnvironmentId.ThornPath,
                    _ => Table.TableEnvironmentId.RiftDepth
                },
                IsElite = node.Type == MapNodeType.Elite,
                VictoryCoinReward = node.Type == MapNodeType.Elite ? 12 : 8
            };
            Identity.ApplyToConfig(normal);
            return normal;
        }

        public void SyncDeckFromEncounter()
        {
            if (ActiveEncounter == null)
            {
                return;
            }

            var snapshot = ActiveEncounter.CollectRunDeckSnapshot();
            RunDeck.Clear();
            RunDeck.AddRange(snapshot);
        }
    }
}
