using System.Linq;
using NUnit.Framework;
using PasserCard.Cards;
using PasserCard.Enchantments;
using PasserCard.Poker;

namespace PasserCard.Tests
{
    public sealed class EnchantmentRulesTests
    {
        [Test]
        public void FamineFold_BlocksPass()
        {
            var card = PlayingCardFactory.Create(Suit.Spades, Rank.Five, Rank.Nine, enchantment: EnchantmentType.FamineFold);
            Assert.IsFalse(EnchantmentRules.CanPass(card));
        }

        [Test]
        public void DeathWither_DoublesMultOnFinalQuota()
        {
            var cards = new[]
            {
                PlayingCardFactory.Create(Suit.Spades, Rank.Ace, Rank.Ace, enchantment: EnchantmentType.DeathWither)
            };

            var mult = EnchantmentRules.GetScoreMultModifier(cards, currentQuota: 4, totalQuotas: 4);
            Assert.AreEqual(2f, mult);
        }

        [Test]
        public void PlagueSpread_InfectsNeighbors()
        {
            var left = PlayingCardFactory.Create(Suit.Clubs, Rank.Two, Rank.Two);
            var center = PlayingCardFactory.Create(Suit.Hearts, Rank.Five, Rank.Five, enchantment: EnchantmentType.PlagueBlight);
            var right = PlayingCardFactory.Create(Suit.Diamonds, Rank.Seven, Rank.Seven);
            var hand = new[] { left, center, right };

            EnchantmentRules.OnPass(center, hand, 1);
            Assert.IsTrue(left.IsPlagued);
            Assert.IsTrue(right.IsPlagued);
        }
    }
}
