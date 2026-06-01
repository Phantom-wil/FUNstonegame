using System.Collections.Generic;

namespace PasserCard.Poker
{
    public static class HandScoreTable
    {
        private static readonly Dictionary<HandCategory, float> CategoryMult = new()
        {
            { HandCategory.HighCard, 1f },
            { HandCategory.Pair, 2f },
            { HandCategory.TwoPair, 2f },
            { HandCategory.ThreeOfAKind, 3f },
            { HandCategory.Straight, 4f },
            { HandCategory.Flush, 4f },
            { HandCategory.FullHouse, 4f },
            { HandCategory.FourOfAKind, 7f },
            { HandCategory.StraightFlush, 8f },
            { HandCategory.RoyalFlush, 8f }
        };

        private static readonly Dictionary<HandCategory, int> CategoryBaseChips = new()
        {
            { HandCategory.HighCard, 5 },
            { HandCategory.Pair, 10 },
            { HandCategory.TwoPair, 20 },
            { HandCategory.ThreeOfAKind, 30 },
            { HandCategory.Straight, 30 },
            { HandCategory.Flush, 35 },
            { HandCategory.FullHouse, 40 },
            { HandCategory.FourOfAKind, 60 },
            { HandCategory.StraightFlush, 100 },
            { HandCategory.RoyalFlush, 100 }
        };

        public static float GetMult(HandCategory category) => CategoryMult[category];

        public static int GetBaseChips(HandCategory category) => CategoryBaseChips[category];
    }
}
