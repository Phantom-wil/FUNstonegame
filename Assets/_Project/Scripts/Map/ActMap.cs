using System.Collections.Generic;
using System.Linq;

namespace PasserCard.Map
{
    public sealed class ActMap
    {
        public ActMap(int act, IReadOnlyList<MapNode> nodes, int startNodeId, int bossNodeId)
        {
            Act = act;
            Nodes = nodes;
            StartNodeId = startNodeId;
            BossNodeId = bossNodeId;
        }

        public int Act { get; }
        public IReadOnlyList<MapNode> Nodes { get; }
        public int StartNodeId { get; }
        public int BossNodeId { get; }

        public MapNode? GetNode(int id) => Nodes.FirstOrDefault(n => n.Id == id);

        public IEnumerable<MapNode> GetAvailableNodes() => Nodes.Where(n => n.IsAvailable);

        public void MarkVisited(int nodeId)
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var node = Nodes[i];
                node.IsCurrent = node.Id == nodeId;
                if (node.Id == nodeId)
                {
                    node.IsVisited = true;
                    node.IsAvailable = false;
                }
                else
                {
                    node.IsAvailable = false;
                }
            }

            var current = GetNode(nodeId);
            if (current == null)
            {
                return;
            }

            for (var i = 0; i < current.Connections.Count; i++)
            {
                var next = GetNode(current.Connections[i]);
                if (next != null && !next.IsVisited)
                {
                    next.IsAvailable = true;
                }
            }
        }

        public void InitializeAvailability()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].IsAvailable = false;
                Nodes[i].IsCurrent = false;
            }

            var start = GetNode(StartNodeId);
            if (start != null)
            {
                start.IsAvailable = true;
                start.IsCurrent = true;
            }
        }
    }
}
