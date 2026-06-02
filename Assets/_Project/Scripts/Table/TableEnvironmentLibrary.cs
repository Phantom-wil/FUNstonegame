using UnityEngine;

namespace PasserCard.Table
{
    public readonly struct TableEnvironmentVisual
    {
        public TableEnvironmentVisual(
            TableEnvironmentId id,
            string displayName,
            string ruleSummary,
            Color baseColor,
            Color accentColor,
            Color playAreaColor,
            Color fogColor,
            float noiseScale,
            float scrollSpeed,
            float vignettePower,
            float pulseSpeed,
            bool useFogOverlay,
            bool useShimmer,
            bool useCrackPulse)
        {
            Id = id;
            DisplayName = displayName;
            RuleSummary = ruleSummary;
            BaseColor = baseColor;
            AccentColor = accentColor;
            PlayAreaColor = playAreaColor;
            FogColor = fogColor;
            NoiseScale = noiseScale;
            ScrollSpeed = scrollSpeed;
            VignettePower = vignettePower;
            PulseSpeed = pulseSpeed;
            UseFogOverlay = useFogOverlay;
            UseShimmer = useShimmer;
            UseCrackPulse = useCrackPulse;
        }

        public TableEnvironmentId Id { get; }
        public string DisplayName { get; }
        public string RuleSummary { get; }
        public Color BaseColor { get; }
        public Color AccentColor { get; }
        public Color PlayAreaColor { get; }
        public Color FogColor { get; }
        public float NoiseScale { get; }
        public float ScrollSpeed { get; }
        public float VignettePower { get; }
        public float PulseSpeed { get; }
        public bool UseFogOverlay { get; }
        public bool UseShimmer { get; }
        public bool UseCrackPulse { get; }
    }

    /// <summary>
    /// Visual presets mirroring Balatro's per-blind backdrop + play-area layering.
    /// </summary>
    public static class TableEnvironmentLibrary
    {
        public static TableEnvironmentVisual Get(TableEnvironmentId id) =>
            id switch
            {
                TableEnvironmentId.SmoothIce => SmoothIce,
                TableEnvironmentId.ThornPath => ThornPath,
                TableEnvironmentId.RiftDepth => RiftDepth,
                _ => FogMist
            };

        public static readonly TableEnvironmentVisual FogMist = new(
            TableEnvironmentId.FogMist,
            "迷雾牌桌",
            "每配额 2 个雾槽 · 入雾不可 Pass",
            new Color(0.06f, 0.09f, 0.11f, 1f),
            new Color(0.22f, 0.28f, 0.32f, 1f),
            new Color(0.12f, 0.16f, 0.19f, 0.55f),
            new Color(0.55f, 0.62f, 0.68f, 0.35f),
            noiseScale: 18f,
            scrollSpeed: 0.08f,
            vignettePower: 1.35f,
            pulseSpeed: 0.6f,
            useFogOverlay: true,
            useShimmer: false,
            useCrackPulse: false);

        public static readonly TableEnvironmentVisual SmoothIce = new(
            TableEnvironmentId.SmoothIce,
            "光滑牌桌",
            "Pass 时有概率额外翻转一次",
            new Color(0.05f, 0.08f, 0.14f, 1f),
            new Color(0.35f, 0.55f, 0.72f, 1f),
            new Color(0.18f, 0.32f, 0.48f, 0.45f),
            new Color(0.7f, 0.85f, 0.95f, 0.2f),
            noiseScale: 24f,
            scrollSpeed: 0.14f,
            vignettePower: 1.2f,
            pulseSpeed: 1.1f,
            useFogOverlay: false,
            useShimmer: true,
            useCrackPulse: false);

        public static readonly TableEnvironmentVisual ThornPath = new(
            TableEnvironmentId.ThornPath,
            "荆棘牌桌",
            "Pass 耗魂币 · 未 Pass 出牌扣魂币",
            new Color(0.1f, 0.05f, 0.05f, 1f),
            new Color(0.45f, 0.18f, 0.12f, 1f),
            new Color(0.28f, 0.12f, 0.1f, 0.5f),
            new Color(0.35f, 0.12f, 0.08f, 0.25f),
            noiseScale: 32f,
            scrollSpeed: 0.05f,
            vignettePower: 1.5f,
            pulseSpeed: 0.45f,
            useFogOverlay: false,
            useShimmer: false,
            useCrackPulse: false);

        public static readonly TableEnvironmentVisual RiftDepth = new(
            TableEnvironmentId.RiftDepth,
            "裂隙牌桌",
            "裂痕附魔触发概率提升",
            new Color(0.04f, 0.03f, 0.08f, 1f),
            new Color(0.55f, 0.22f, 0.75f, 1f),
            new Color(0.22f, 0.1f, 0.32f, 0.52f),
            new Color(0.45f, 0.15f, 0.55f, 0.3f),
            noiseScale: 20f,
            scrollSpeed: 0.1f,
            vignettePower: 1.45f,
            pulseSpeed: 1.4f,
            useFogOverlay: false,
            useShimmer: false,
            useCrackPulse: true);
    }
}
