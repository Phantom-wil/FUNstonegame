using PasserCard.Cards;
using PasserCard.Encounter;

namespace PasserCard.Run
{
    public readonly struct RunIdentityDefinition
    {
        public RunIdentityDefinition(
            RunIdentityId id,
            string displayName,
            string summary,
            int startingCoins,
            float charonDiscount,
            int extraPasses,
            float scoreMultiplier,
            float coinGainMultiplier,
            float shopPriceMultiplier)
        {
            Id = id;
            DisplayName = displayName;
            Summary = summary;
            StartingCoins = startingCoins;
            CharonDiscount = charonDiscount;
            ExtraPasses = extraPasses;
            ScoreMultiplier = scoreMultiplier;
            CoinGainMultiplier = coinGainMultiplier;
            ShopPriceMultiplier = shopPriceMultiplier;
        }

        public RunIdentityId Id { get; }
        public string DisplayName { get; }
        public string Summary { get; }
        public int StartingCoins { get; }
        public float CharonDiscount { get; }
        public int ExtraPasses { get; }
        public float ScoreMultiplier { get; }
        public float CoinGainMultiplier { get; }
        public float ShopPriceMultiplier { get; }

        public void ApplyToConfig(EncounterConfig config)
        {
            config.IdentityExtraPasses = ExtraPasses;
            config.IdentityScoreMultiplier = ScoreMultiplier;
            config.IdentityCoinGainMultiplier = CoinGainMultiplier;
        }
    }

    public static class RunIdentityLibrary
    {
        public static RunIdentityDefinition Get(RunIdentityId id) =>
            id switch
            {
                RunIdentityId.SoulSplitter => SoulSplitter,
                RunIdentityId.CoinKeeper => CoinKeeper,
                _ => Ferryman
            };

        public static readonly RunIdentityDefinition Ferryman = new(
            RunIdentityId.Ferryman,
            "摆渡者",
            "卡戎献祭费用 -10%",
            startingCoins: 20,
            charonDiscount: 0.1f,
            extraPasses: 0,
            scoreMultiplier: 1f,
            coinGainMultiplier: 1f,
            shopPriceMultiplier: 1f);

        public static readonly RunIdentityDefinition SoulSplitter = new(
            RunIdentityId.SoulSplitter,
            "裂魂者",
            "Pass 次数 +1 · 战争裂痕分裂概率提升",
            startingCoins: 18,
            charonDiscount: 0f,
            extraPasses: 1,
            scoreMultiplier: 1f,
            coinGainMultiplier: 1f,
            shopPriceMultiplier: 1f);

        public static readonly RunIdentityDefinition CoinKeeper = new(
            RunIdentityId.CoinKeeper,
            "守财灵",
            "魂币获取 +15% · 商店涨价 +10%",
            startingCoins: 24,
            charonDiscount: 0f,
            extraPasses: 0,
            scoreMultiplier: 1f,
            coinGainMultiplier: 1.15f,
            shopPriceMultiplier: 1.1f);
    }
}
