using System;
using System.Collections.Generic;
using PasserCard.Enchantments;

namespace PasserCard.Cards
{
    public static class PlayingCardFactory
    {
        public static PlayingCardInstance Create(
            Suit suit,
            Rank cornerA,
            Rank cornerB,
            PlagueSuit plague = PlagueSuit.None,
            EnchantmentType enchantment = EnchantmentType.None)
        {
            return new PlayingCardInstance(Guid.NewGuid(), suit, cornerA, cornerB, plagueSuit: plague, enchantment: enchantment);
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
                    var cornerB = !dualRank ? rank : RankExtensions.ClampRank(rankValue + rng.Next(-2, 3));
                    deck.Add(Create(suit, rank, cornerB));
                }
            }

            return deck;
        }

        public static List<PlayingCardInstance> CreateDeckFromTemplate(IEnumerable<PlayingCardTemplate> templates)
        {
            var deck = new List<PlayingCardInstance>();
            foreach (var template in templates)
            {
                deck.Add(Create(
                    template.Suit,
                    template.CornerA,
                    template.CornerB,
                    template.PlagueSuit,
                    template.Enchantment));
            }

            return deck;
        }
    }

    public readonly struct PlayingCardTemplate
    {
        public PlayingCardTemplate(
            Suit suit,
            Rank cornerA,
            Rank cornerB,
            PlagueSuit plagueSuit = PlagueSuit.None,
            EnchantmentType enchantment = EnchantmentType.None)
        {
            Suit = suit;
            CornerA = cornerA;
            CornerB = cornerB;
            PlagueSuit = plagueSuit;
            Enchantment = enchantment;
        }

        public Suit Suit { get; }
        public Rank CornerA { get; }
        public Rank CornerB { get; }
        public PlagueSuit PlagueSuit { get; }
        public EnchantmentType Enchantment { get; }
    }
}
