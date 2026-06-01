using System;
using System.Collections.Generic;

namespace PasserCard.Cards
{
    public static class PlayingCardFactory
    {
        public static PlayingCardInstance Create(Suit suit, Rank cornerA, Rank cornerB, PlagueSuit plague = PlagueSuit.None)
        {
            return new PlayingCardInstance(Guid.NewGuid(), suit, cornerA, cornerB, plagueSuit: plague);
        }

        public static List<PlayingCardInstance> CreateStandardDeck(bool dualRank = false, Random? random = null)
        {
            var rng = random ?? new Random();
            var deck = new List<PlayingCardInstance>(52);

            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                for (var rankValue = (int)Rank.Two; rankValue <= (int)Rank.Ace; rankValue++)
                {
                    var rank = (Rank)rankValue;
                    Rank cornerB;

                    if (!dualRank)
                    {
                        cornerB = rank;
                    }
                    else
                    {
                        var offset = rng.Next(-2, 3);
                        cornerB = RankExtensions.ClampRank(rankValue + offset);
                    }

                    deck.Add(Create(suit, rank, cornerB));
                }
            }

            return deck;
        }
    }
}
