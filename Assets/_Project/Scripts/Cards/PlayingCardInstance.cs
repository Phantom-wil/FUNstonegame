using System;
using PasserCard.Enchantments;

namespace PasserCard.Cards
{
    [Serializable]
    public sealed class PlayingCardInstance
    {
        public PlayingCardInstance(
            Guid instanceId,
            Suit suit,
            Rank cornerA,
            Rank cornerB,
            bool isFlipped = false,
            PlagueSuit plagueSuit = PlagueSuit.None,
            EnchantmentType enchantment = EnchantmentType.None,
            CardSplitState splitState = CardSplitState.Full,
            bool isFaceLocked = false)
        {
            InstanceId = instanceId == Guid.Empty ? Guid.NewGuid() : instanceId;
            Suit = suit;
            CornerA = cornerA;
            CornerB = cornerB;
            IsFlipped = isFlipped;
            PlagueSuit = plagueSuit;
            Enchantment = enchantment;
            SplitState = splitState;
            IsFaceLocked = isFaceLocked;
        }

        public Guid InstanceId { get; }
        public Suit Suit { get; private set; }
        public Rank CornerA { get; private set; }
        public Rank CornerB { get; private set; }
        public bool IsFlipped { get; private set; }
        public PlagueSuit PlagueSuit { get; private set; }
        public EnchantmentType Enchantment { get; private set; }
        public CardSplitState SplitState { get; set; }
        public bool IsFaceLocked { get; private set; }

        public bool WasPassedThisQuota { get; set; }
        public bool IsPinnedToFogSlot { get; set; }
        public bool PassEffectsSuppressed { get; set; }

        public Rank EffectiveRank
        {
            get
            {
                var rank = IsFlipped ? CornerB : CornerA;
                if (SplitState == CardSplitState.HalfLeft || SplitState == CardSplitState.HalfRight)
                {
                    return RankExtensions.ClampRank(Math.Max(2, rank.ToSortValue() / 2));
                }

                return rank;
            }
        }

        public bool IsPlagued => PlagueSuit != PlagueSuit.None;

        public void TogglePassFace()
        {
            if (IsFaceLocked)
            {
                return;
            }

            IsFlipped = !IsFlipped;
        }

        public void SetFace(bool useCornerB) => IsFlipped = useCornerB;

        public void ApplyEnchantment(EnchantmentType enchantment)
        {
            Enchantment = enchantment;
            if (enchantment == EnchantmentType.FamineFold)
            {
                IsFaceLocked = true;
            }
        }

        public void ApplyPlagueSuit(PlagueSuit plague)
        {
            PlagueSuit = plague;
            if (plague != PlagueSuit.None)
            {
                Suit = SuitExtensions.FromPlagueVariant(plague);
            }
        }

        public void ResetQuotaFlags()
        {
            WasPassedThisQuota = false;
            IsPinnedToFogSlot = false;
            PassEffectsSuppressed = false;
        }

        public PlayingCardInstance CloneShallow()
        {
            return new PlayingCardInstance(
                Guid.NewGuid(),
                Suit,
                CornerA,
                CornerB,
                IsFlipped,
                PlagueSuit,
                Enchantment,
                SplitState,
                IsFaceLocked);
        }
    }
}
