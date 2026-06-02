using System.Collections.Generic;
using PasserCard.Boss;

namespace PasserCard.Map
{
    public sealed class MapNode
    {
        public MapNode(int id, int layer, MapNodeType type, BossId bossId = BossId.None)
        {
            Id = id;
            Layer = layer;
            Type = type;
            BossId = bossId;
            Connections = new List<int>();
        }

        public int Id { get; }
        public int Layer { get; }
        public MapNodeType Type { get; }
        public BossId BossId { get; }
        public List<int> Connections { get; }
        public bool IsVisited { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsCurrent { get; set; }

        public string DisplayLabel => Type switch
        {
            MapNodeType.Start => "起点",
            MapNodeType.Normal => "守路人",
            MapNodeType.Elite => "精英",
            MapNodeType.Shop => "商店",
            MapNodeType.Planet => "星球",
            MapNodeType.Charon => "卡戎",
            MapNodeType.Boss => BossId == BossId.None ? "Boss" : BossLibrary.Get(BossId).DisplayName,
            _ => "?"
        };
    }
}
