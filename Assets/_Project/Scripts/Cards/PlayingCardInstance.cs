using System;

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
            PlagueSuit plagueSuit = PlagueSuit.None)
        {
            InstanceId = instanceId == Guid.Empty ? Guid.NewGuid() : instanceId;
            Suit = suit;
            CornerA = cornerA;
            CornerB = cornerB;
            IsFlipped = isFlipped;
            PlagueSuit = plagueSuit;
        }

        public Guid InstanceId { get; }
        public Suit Suit { get; private set; }
        public Rank CornerA { get; private set; }
        public Rank CornerB { get; private set; }
        public bool IsFlipped { get; private set; }
        public PlagueSuit PlagueSuit { get; private set; }

        public bool WasPassedThisQuota { get; set; }
        public bool IsPinnedToFogSlot { get; set; }
        public bool PassEffectsSuppressed { get; set; }

        public Rank EffectiveRank => IsFlipped ? CornerB : CornerA;

        public bool IsPlagued => PlagueSuit != PlagueSuit.None;

        public void TogglePassFace() => IsFlipped = !IsFlipped;

        public void SetFace(bool useCornerB) => IsFlipped = useCornerB;

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
    }
}
