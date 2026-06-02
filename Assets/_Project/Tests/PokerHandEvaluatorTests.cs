using System.Collections.Generic;
using NUnit.Framework;
using PasserCard.Cards;
using PasserCard.Poker;

namespace PasserCard.Tests
{
    public sealed class PokerHandEvaluatorTests
    {
        [Test]
        public void Evaluate_Pair_ReturnsPairCategory()
        {
            var cards = new List<ScoringCard>
            {
                new(Rank.King, Suit.Spades, true, true),
                new(Rank.King, Suit.Hearts, true, true),
                new(Rank.Five, Suit.Clubs, true, true)
            };

            var result = PokerHandEvaluator.Evaluate(cards);

            Assert.AreEqual(HandCategory.Pair, result.Category);
            Assert.Greater(result.TotalScore, 0);
        }

        [Test]
        public void Evaluate_SpadesPassModifier_ExcludesStraight()
        {
            var cards = new List<ScoringCard>
            {
                new(Rank.Ten, Suit.Spades, false, true),
                new(Rank.Jack, Suit.Spades, true, true),
                new(Rank.Queen, Suit.Spades, true, true),
                new(Rank.King, Suit.Spades, true, true),
                new(Rank.Ace, Suit.Spades, true, true)
            };

            var category = PokerHandEvaluator.DetectCategory(cards);

            Assert.AreEqual(HandCategory.Flush, category);
        }

        [Test]
        public void Evaluate_Straight_DetectsWheel()
        {
            var cards = new List<ScoringCard>
            {
                new(Rank.Ace, Suit.Hearts, true, true),
                new(Rank.Two, Suit.Hearts, true, true),
                new(Rank.Three, Suit.Hearts, true, true),
                new(Rank.Four, Suit.Hearts, true, true),
                new(Rank.Five, Suit.Hearts, true, true)
            };

            var category = PokerHandEvaluator.DetectCategory(cards);

            Assert.AreEqual(HandCategory.Straight, category);
        }

        [Test]
        public void Evaluate_FullHouse_Detected()
        {
            var cards = new List<ScoringCard>
            {
                new(Rank.King, Suit.Spades, true, true),
                new(Rank.King, Suit.Hearts, true, true),
                new(Rank.King, Suit.Clubs, true, true),
                new(Rank.Five, Suit.Diamonds, true, true),
                new(Rank.Five, Suit.Spades, true, true)
            };

            Assert.AreEqual(HandCategory.FullHouse, PokerHandEvaluator.DetectCategory(cards));
        }

        [Test]
        public void Evaluate_MultModifier_AppliesToTotal()
        {
            var cards = new List<ScoringCard>
            {
                new(Rank.Ace, Suit.Spades, true, true),
                new(Rank.Ace, Suit.Hearts, true, true)
            };

            var normal = PokerHandEvaluator.Evaluate(cards, 1f);
            var boosted = PokerHandEvaluator.Evaluate(cards, 2f);
            Assert.Greater(boosted.TotalScore, normal.TotalScore);
        }
    }
}
