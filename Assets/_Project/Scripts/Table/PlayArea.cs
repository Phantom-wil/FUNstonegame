using System;
using System.Collections.Generic;
using PasserCard.Cards;

namespace PasserCard.Table
{
    public sealed class PlayArea
    {
        public const int SlotCount = 5;

        private readonly PlaySlot[] _slots =
        {
            new(0),
            new(1),
            new(2),
            new(3),
            new(4)
        };

        public IReadOnlyList<PlaySlot> Slots => _slots;

        public void RefreshFog(int fogCount, Random random)
        {
            for (var i = 0; i < _slots.Length; i++)
            {
                _slots[i].SetFog(false);
                _slots[i].ResetQuota();
            }

            fogCount = Math.Clamp(fogCount, 0, SlotCount);
            var indices = new List<int> { 0, 1, 2, 3, 4 };
            for (var i = 0; i < fogCount; i++)
            {
                var pick = random.Next(indices.Count);
                var slotIndex = indices[pick];
                indices.RemoveAt(pick);
                _slots[slotIndex].SetFog(true);
            }
        }

        public bool TryPinToFogSlot(int slotIndex, PlayingCardInstance card)
        {
            if (slotIndex < 0 || slotIndex >= SlotCount)
            {
                return false;
            }

            return _slots[slotIndex].TryPinCard(card);
        }

        public bool TryPlaceCard(int slotIndex, PlayingCardInstance card)
        {
            if (slotIndex < 0 || slotIndex >= SlotCount)
            {
                return false;
            }

            return _slots[slotIndex].TryPlaceCard(card);
        }

        public List<PlayingCardInstance> GetCommittedCards()
        {
            var cards = new List<PlayingCardInstance>();
            for (var i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].Card != null)
                {
                    cards.Add(_slots[i].Card!);
                }
            }

            return cards;
        }

        public void ResetSlots()
        {
            for (var i = 0; i < _slots.Length; i++)
            {
                _slots[i].ResetQuota();
            }
        }
    }
}
