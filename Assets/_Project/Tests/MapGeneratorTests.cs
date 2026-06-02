using System;
using System.Linq;
using NUnit.Framework;
using PasserCard.Boss;
using PasserCard.Map;

namespace PasserCard.Tests
{
    public sealed class MapGeneratorTests
    {
        [Test]
        public void GenerateAct_CreatesConnectedPathToBoss()
        {
            var map = MapGenerator.GenerateAct(1, new Random(42), BossId.Nyx);
            var boss = map.GetNode(map.BossNodeId);

            Assert.NotNull(boss);
            Assert.AreEqual(MapNodeType.Boss, boss!.Type);
            Assert.AreEqual(BossId.Nyx, boss.BossId);
            Assert.Greater(map.Nodes.Count, 5);
        }

        [Test]
        public void MarkVisited_UnlocksConnections()
        {
            var map = MapGenerator.GenerateAct(1, new Random(7), BossId.Nyx);
            var start = map.GetNode(map.StartNodeId)!;
            map.MarkVisited(start.Id);

            var available = map.GetAvailableNodes().ToList();
            Assert.Greater(available.Count, 0);
            foreach (var node in available)
            {
                Assert.IsTrue(start.Connections.Contains(node.Id));
            }
        }
    }
}
