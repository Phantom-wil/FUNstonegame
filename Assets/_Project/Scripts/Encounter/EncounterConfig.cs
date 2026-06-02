using PasserCard.Boss;
using PasserCard.Table;

namespace PasserCard.Encounter
{
    public enum EncounterPhase
    {
        QuotaSetup = 0,
        Discard = 1,
        PrePinFog = 2,
        Pass = 3,
        PlayToSlots = 4,
        ResolveQuota = 5,
        EncounterComplete = 6
    }

    public sealed class EncounterConfig
    {
        public int HandSize { get; set; } = 8;
        public int PlayQuotas { get; set; } = 4;
        public int DiscardsRemaining { get; set; } = 3;
        public int PassLimit { get; set; } = 5;
        public int MistFogSlots { get; set; } = 2;
        public int TargetScore { get; set; } = 120;
        public int RetaliationCoinLoss { get; set; } = 2;
        public int VictoryCoinReward { get; set; } = 8;
        public int DefeatCoinLoss { get; set; } = 5;
        public TableEnvironmentId TableEnvironment { get; set; } = TableEnvironmentId.FogMist;
        public BossId BossId { get; set; } = BossId.None;
        public bool IsElite { get; set; }
        public bool HideFogCorners { get; set; }
        public float IdentityScoreMultiplier { get; set; } = 1f;
        public int IdentityExtraPasses { get; set; }
        public float IdentityCoinGainMultiplier { get; set; } = 1f;
    }
}
