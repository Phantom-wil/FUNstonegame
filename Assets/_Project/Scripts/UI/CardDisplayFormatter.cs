using PasserCard.Cards;

namespace PasserCard.UI
{
    public static class CardDisplayFormatter
    {
        public static string FormatCard(PlayingCardInstance card)
        {
            var suit = SuitSymbol(card.Suit);
            var active = RankLabel(card.EffectiveRank);
            var cornerA = RankLabel(card.CornerA);
            var cornerB = RankLabel(card.CornerB);
            var face = card.IsFlipped ? "B" : "A";
            return $"{suit} {active} ({cornerA}/{cornerB}) {face}";
        }

        public static string FormatCardShort(PlayingCardInstance card)
        {
            return $"{SuitSymbol(card.Suit)}{RankLabel(card.EffectiveRank)}";
        }

        public static string SuitSymbol(Suit suit) =>
            suit switch
            {
                Suit.Spades => "♠",
                Suit.Hearts => "♥",
                Suit.Clubs => "♣",
                Suit.Diamonds => "♦",
                _ => "?"
            };

        public static string RankLabel(Rank rank) =>
            rank switch
            {
                Rank.Ace => "A",
                Rank.King => "K",
                Rank.Queen => "Q",
                Rank.Jack => "J",
                Rank.Ten => "10",
                _ => ((int)rank).ToString()
            };
    }
}
