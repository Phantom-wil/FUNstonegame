using System;
using System.Collections.Generic;
using PasserCard.Cards;

namespace PasserCard.Enchantments
{
    public static class EnchantmentRules
    {
        public static bool CanPass(PlayingCardInstance card)
        {
            return card.Enchantment != EnchantmentType.FamineFold || !card.IsFaceLocked;
        }

        public static void OnPass(PlayingCardInstance card, IReadOnlyList<PlayingCardInstance> hand, int handIndex)
        {
            if (card.Enchantment == EnchantmentType.PlagueBlight)
            {
                SpreadPlague(hand, handIndex);
            }
        }

        public static float GetScoreMultModifier(IReadOnlyList<PlayingCardInstance> committed, int currentQuota, int totalQuotas)
        {
            var mult = 1f;
            var hasWither = false;
            for (var i = 0; i < committed.Count; i++)
            {
                if (committed[i].Enchantment == EnchantmentType.DeathWither)
                {
                    hasWither = true;
                    break;
                }
            }

            if (hasWither && currentQuota >= totalQuotas)
            {
                mult *= 2f;
            }

            return mult;
        }

        public static bool ShouldBurnAfterPlay(PlayingCardInstance card) =>
            card.Enchantment == EnchantmentType.DeathWither;

        public static bool TrySplitWarCrack(PlayingCardInstance card, Random random, out PlayingCardInstance? half)
        {
            half = null;
            if (card.Enchantment != EnchantmentType.WarCrack || card.SplitState != CardSplitState.Full)
            {
                return false;
            }

            if (random.NextDouble() > 0.35d)
            {
                return false;
            }

            card.SplitState = CardSplitState.HalfLeft;
            var halfRank = RankExtensions.ClampRank(Math.Max(2, card.EffectiveRank.ToSortValue() / 2));
            half = new PlayingCardInstance(
                Guid.NewGuid(),
                card.Suit,
                halfRank,
                halfRank,
                card.IsFlipped,
                card.PlagueSuit)
            {
                SplitState = CardSplitState.HalfRight
            };
            return true;
        }

        private static void SpreadPlague(IReadOnlyList<PlayingCardInstance> hand, int sourceIndex)
        {
            TryInfectNeighbor(hand, sourceIndex - 1);
            TryInfectNeighbor(hand, sourceIndex + 1);
        }

        private static void TryInfectNeighbor(IReadOnlyList<PlayingCardInstance> hand, int index)
        {
            if (index < 0 || index >= hand.Count)
            {
                return;
            }

            var neighbor = hand[index];
            if (neighbor.Enchantment == EnchantmentType.PlagueBlight)
            {
                return;
            }

            neighbor.ApplyPlagueSuit(neighbor.Suit.ToPlagueVariant());
        }
    }
}
