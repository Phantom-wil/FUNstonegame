using System;
using System.Collections.Generic;
using PasserCard.Cards;

namespace PasserCard.Gameplay
{
    public sealed class Hand
    {
        private readonly List<PlayingCardInstance> _cards = new();

        public Hand(int maxSize = 10)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSize));
            }

            MaxSize = maxSize;
        }

        public int MaxSize { get; }
        public IReadOnlyList<PlayingCardInstance> Cards => _cards;
        public int Count => _cards.Count;
        public bool IsFull => _cards.Count >= MaxSize;

        public bool TryAdd(PlayingCardInstance card)
        {
            if (card == null || IsFull)
            {
                return false;
            }

            _cards.Add(card);
            return true;
        }

        public bool TryRemove(Guid instanceId, out PlayingCardInstance card)
        {
            for (var i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].InstanceId != instanceId)
                {
                    continue;
                }

                card = _cards[i];
                _cards.RemoveAt(i);
                return true;
            }

            card = null!;
            return false;
        }

        public bool Contains(Guid instanceId)
        {
            for (var i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].InstanceId == instanceId)
                {
                    return true;
                }
            }

            return false;
        }

        public void Clear() => _cards.Clear();
    }
}
