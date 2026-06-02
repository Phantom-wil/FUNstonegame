using PasserCard.Encounter;
using PasserCard.Table;

namespace PasserCard.Boss
{
    public readonly struct BossDefinition
    {
        public BossDefinition(
            BossId id,
            string displayName,
            string ruleSummary,
            int targetScore,
            int mistFogSlots,
            int playQuotas,
            int passLimit,
            int retaliationCoinLoss,
            TableEnvironmentId tableEnvironment,
            bool hideFogCorners)
        {
            Id = id;
            DisplayName = displayName;
            RuleSummary = ruleSummary;
            TargetScore = targetScore;
            MistFogSlots = mistFogSlots;
            PlayQuotas = playQuotas;
            PassLimit = passLimit;
            RetaliationCoinLoss = retaliationCoinLoss;
            TableEnvironment = tableEnvironment;
            HideFogCorners = hideFogCorners;
        }

        public BossId Id { get; }
        public string DisplayName { get; }
        public string RuleSummary { get; }
        public int TargetScore { get; }
        public int MistFogSlots { get; }
        public int PlayQuotas { get; }
        public int PassLimit { get; }
        public int RetaliationCoinLoss { get; }
        public TableEnvironmentId TableEnvironment { get; }
        public bool HideFogCorners { get; }

        public EncounterConfig ToEncounterConfig(bool isElite = false)
        {
            return new EncounterConfig
            {
                TargetScore = isElite ? (int)(TargetScore * 1.15f) : TargetScore,
                MistFogSlots = MistFogSlots,
                PlayQuotas = PlayQuotas,
                PassLimit = PassLimit,
                RetaliationCoinLoss = RetaliationCoinLoss,
                TableEnvironment = TableEnvironment,
                BossId = Id,
                IsElite = isElite,
                HideFogCorners = HideFogCorners
            };
        }
    }

    public static class BossLibrary
    {
        public static BossDefinition Get(BossId id) =>
            id switch
            {
                BossId.Hydra => Hydra,
                BossId.Cerberus => Cerberus,
                BossId.Styx => Styx,
                BossId.Nyx => Nyx,
                _ => Nyx
            };

        public static readonly BossDefinition Nyx = new(
            BossId.Nyx,
            "尼克斯",
            "Boss · 3 雾槽 · 雾槽内牌隐藏非生效角",
            targetScore: 160,
            mistFogSlots: 3,
            playQuotas: 4,
            passLimit: 5,
            retaliationCoinLoss: 3,
            TableEnvironmentId.FogMist,
            hideFogCorners: true);

        public static readonly BossDefinition Hydra = new(
            BossId.Hydra,
            "许德拉",
            "Boss · 6 出牌配额 · 未达标反击更重",
            targetScore: 180,
            mistFogSlots: 2,
            playQuotas: 6,
            passLimit: 5,
            retaliationCoinLoss: 4,
            TableEnvironmentId.ThornPath,
            hideFogCorners: false);

        public static readonly BossDefinition Cerberus = new(
            BossId.Cerberus,
            "刻耳柏洛斯",
            "Boss · Pass 上限 3 · 每配额需至少 Pass 一次",
            targetScore: 200,
            mistFogSlots: 2,
            playQuotas: 4,
            passLimit: 3,
            retaliationCoinLoss: 3,
            TableEnvironmentId.RiftDepth,
            hideFogCorners: false);

        public static readonly BossDefinition Styx = new(
            BossId.Styx,
            "斯提克斯",
            "Boss · 光滑牌桌 · Pass 后额外翻转概率提升",
            targetScore: 220,
            mistFogSlots: 2,
            playQuotas: 4,
            passLimit: 4,
            retaliationCoinLoss: 3,
            TableEnvironmentId.SmoothIce,
            hideFogCorners: false);
    }
}
