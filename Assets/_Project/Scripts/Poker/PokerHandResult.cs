using PasserCard.Cards;

namespace PasserCard.Poker
{
    public readonly struct ScoringCard
    {
        public ScoringCard(Rank rank, Suit suit, bool countsForStraight, bool countsForFlush, int bonusChips = 0)
        {
            Rank = rank;
            Suit = suit;
            CountsForStraight = countsForStraight;
            CountsForFlush = countsForFlush;
            BonusChips = bonusChips;
        }

        public Rank Rank { get; }
        public Suit Suit { get; }
        public bool CountsForStraight { get; }
        public bool CountsForFlush { get; }
        public int BonusChips { get; }
    }

    public readonly struct PokerHandResult
    {
        public PokerHandResult(HandCategory category, int chips, float mult, int totalScore)
        {
            Category = category;
            Chips = chips;
            Mult = mult;
            TotalScore = totalScore;
        }

        public HandCategory Category { get; }
        public int Chips { get; }
        public float Mult { get; }
        public int TotalScore { get; }
    }
}
