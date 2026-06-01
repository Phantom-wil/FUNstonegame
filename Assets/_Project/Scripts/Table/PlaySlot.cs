using PasserCard.Cards;

namespace PasserCard.Table
{
    public enum PlaySlotKind
    {
        Clear = 0,
        Fog = 1
    }

    public sealed class PlaySlot
    {
        public PlaySlot(int index)
        {
            Index = index;
        }

        public int Index { get; }
        public PlaySlotKind Kind { get; private set; } = PlaySlotKind.Clear;
        public PlayingCardInstance? Card { get; private set; }

        public bool IsEmpty => Card == null;
        public bool IsFog => Kind == PlaySlotKind.Fog;

        public void SetFog(bool fog) => Kind = fog ? PlaySlotKind.Fog : PlaySlotKind.Clear;

        public bool TryPinCard(PlayingCardInstance card)
        {
            if (Card != null || !IsFog)
            {
                return false;
            }

            Card = card;
            card.IsPinnedToFogSlot = true;
            card.PassEffectsSuppressed = true;
            return true;
        }

        public bool TryPlaceCard(PlayingCardInstance card)
        {
            if (Card != null)
            {
                return false;
            }

            if (IsFog && card.WasPassedThisQuota)
            {
                return false;
            }

            Card = card;
            if (IsFog)
            {
                card.PassEffectsSuppressed = true;
            }

            return true;
        }

        public PlayingCardInstance? ClearCard()
        {
            var existing = Card;
            if (existing == null)
            {
                return null;
            }

            Card = null;
            return existing;
        }

        public void ResetQuota()
        {
            Card = null;
        }
    }
}
