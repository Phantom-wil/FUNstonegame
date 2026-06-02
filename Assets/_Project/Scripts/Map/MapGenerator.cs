using System;
using System.Collections.Generic;
using PasserCard.Boss;

namespace PasserCard.Map
{
    public static class MapGenerator
    {
        private static readonly MapNodeType[] CombatPool =
        {
            MapNodeType.Normal,
            MapNodeType.Normal,
            MapNodeType.Elite,
            MapNodeType.Planet,
            MapNodeType.Shop
        };

        public static ActMap GenerateAct(int act, Random random, BossId bossId)
        {
            const int layers = 8;
            const int nodesPerLayer = 3;
            var nodes = new List<MapNode>();
            var layerNodeIds = new List<int>[layers + 1];
            var nextId = 0;

            for (var layer = 0; layer <= layers; layer++)
            {
                var count = layer == 0 || layer == layers ? 1 : nodesPerLayer;
                layerNodeIds[layer] = new List<int>(count);
                for (var i = 0; i < count; i++)
                {
                    MapNodeType type;
                    if (layer == 0)
                    {
                        type = MapNodeType.Start;
                    }
                    else if (layer == layers)
                    {
                        type = MapNodeType.Boss;
                    }
                    else if (layer == layers / 2 && i == 1)
                    {
                        type = MapNodeType.Charon;
                    }
                    else
                    {
                        type = CombatPool[random.Next(CombatPool.Length)];
                    }

                    var node = new MapNode(nextId++, layer, type, layer == layers ? bossId : BossId.None);
                    nodes.Add(node);
                    layerNodeIds[layer].Add(node.Id);
                }
            }

            for (var layer = 0; layer < layers; layer++)
            {
                var current = layerNodeIds[layer];
                var next = layerNodeIds[layer + 1];
                for (var i = 0; i < current.Count; i++)
                {
                    var from = nodes[current[i]];
                    var targets = new HashSet<int>();
                    targets.Add(next[Math.Min(i, next.Count - 1)]);
                    if (next.Count > 1)
                    {
                        targets.Add(next[random.Next(next.Count)]);
                    }

                    foreach (var target in targets)
                    {
                        if (!from.Connections.Contains(target))
                        {
                            from.Connections.Add(target);
                        }
                    }
                }
            }

            var startId = layerNodeIds[0][0];
            var bossNodeId = layerNodeIds[layers][0];
            var map = new ActMap(act, nodes, startId, bossNodeId);
            map.InitializeAvailability();
            return map;
        }
    }
}
