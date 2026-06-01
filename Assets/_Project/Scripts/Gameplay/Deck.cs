using System;
using System.Collections.Generic;
using PasserCard.Cards;

namespace PasserCard.Gameplay
{
    public sealed class Deck
    {
        private readonly List<PlayingCardInstance> _cards = new();
        private readonly Random _random = new();

        public IReadOnlyList<PlayingCardInstance> Cards => _cards;
        public int Count => _cards.Count;

        public void Add(PlayingCardInstance card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            _cards.Add(card);
        }

        public void AddRange(IEnumerable<PlayingCardInstance> cards)
        {
            if (cards == null)
            {
                throw new ArgumentNullException(nameof(cards));
            }

            foreach (var card in cards)
            {
                Add(card);
            }
        }

        public void Shuffle()
        {
            for (var i = _cards.Count - 1; i > 0; i--)
            {
                var j = _random.Next(i + 1);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }

        public bool TryDraw(out PlayingCardInstance card)
        {
            if (_cards.Count == 0)
            {
                card = null!;
                return false;
            }

            var lastIndex = _cards.Count - 1;
            card = _cards[lastIndex];
            _cards.RemoveAt(lastIndex);
            return true;
        }

        public void Clear() => _cards.Clear();
    }
}
