using System;
using System.Collections.Generic;
using System.Linq;
using PasserCard.Cards;

namespace PasserCard.Poker
{
    public static class PokerHandEvaluator
    {
        public static PokerHandResult Evaluate(IReadOnlyList<ScoringCard> cards, float multModifier = 1f)
        {
            if (cards == null || cards.Count == 0)
            {
                throw new ArgumentException("At least one card is required.", nameof(cards));
            }

            if (cards.Count > 5)
            {
                throw new ArgumentException("At most five cards can be scored.", nameof(cards));
            }

            var category = DetectCategory(cards);
            var rankChips = 0;
            for (var i = 0; i < cards.Count; i++)
            {
                rankChips += cards[i].Rank.ToSortValue();
                rankChips += cards[i].BonusChips;
            }

            var chips = HandScoreTable.GetBaseChips(category) + rankChips;
            var mult = HandScoreTable.GetMult(category) * multModifier;
            var total = (int)Math.Round(chips * mult);
            return new PokerHandResult(category, chips, mult, total);
        }

        public static HandCategory DetectCategory(IReadOnlyList<ScoringCard> cards)
        {
            var flushCards = cards.Where(c => c.CountsForFlush).ToList();
            var straightCards = cards.Where(c => c.CountsForStraight).ToList();

            var isFlush = flushCards.Count == cards.Count && cards.Count >= 5 && flushCards.Select(c => c.Suit).Distinct().Count() == 1;
            var straightRanks = isFlush ? flushCards : straightCards;
            var isStraight = straightRanks.Count >= 5 && IsStraight(straightRanks.Select(c => c.Rank).ToList());

            if (isFlush && isStraight)
            {
                var ranks = flushCards.Select(c => c.Rank).OrderByDescending(r => r.ToSortValue()).ToList();
                if (ranks[0] == Rank.Ace && ranks[1] == Rank.King)
                {
                    return HandCategory.RoyalFlush;
                }

                return HandCategory.StraightFlush;
            }

            var rankGroups = cards
                .GroupBy(c => c.Rank)
                .Select(g => new { Rank = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ThenByDescending(g => g.Rank.ToSortValue())
                .ToList();

            if (rankGroups.Count > 0 && rankGroups[0].Count == 4)
            {
                return HandCategory.FourOfAKind;
            }

            if (rankGroups.Count >= 2 && rankGroups[0].Count == 3 && rankGroups[1].Count == 2)
            {
                return HandCategory.FullHouse;
            }

            if (isFlush)
            {
                return HandCategory.Flush;
            }

            if (isStraight)
            {
                return HandCategory.Straight;
            }

            if (rankGroups.Count > 0 && rankGroups[0].Count == 3)
            {
                return HandCategory.ThreeOfAKind;
            }

            if (rankGroups.Count >= 2 && rankGroups[0].Count == 2 && rankGroups[1].Count == 2)
            {
                return HandCategory.TwoPair;
            }

            if (rankGroups.Count > 0 && rankGroups[0].Count == 2)
            {
                return HandCategory.Pair;
            }

            return HandCategory.HighCard;
        }

        private static bool IsStraight(List<Rank> ranks)
        {
            if (ranks.Count < 5)
            {
                return false;
            }

            var values = ranks.Select(r => r.ToSortValue()).Distinct().OrderBy(v => v).ToList();
            if (values.Count < 5)
            {
                return false;
            }

            for (var start = 0; start <= values.Count - 5; start++)
            {
                var sequential = true;
                for (var i = 1; i < 5; i++)
                {
                    if (values[start + i] != values[start] + i)
                    {
                        sequential = false;
                        break;
                    }
                }

                if (sequential)
                {
                    return true;
                }
            }

            var wheel = new[] { 2, 3, 4, 5, 14 };
            return wheel.All(w => values.Contains(w));
        }
    }
}
