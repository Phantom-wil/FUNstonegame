using System;
using System.Collections.Generic;
using PasserCard.Cards;
using PasserCard.Enchantments;

namespace PasserCard.Run
{
    public readonly struct StartingDeckDefinition
    {
        public StartingDeckDefinition(StartingDeckId id, string displayName, string summary, bool dualRankBias)
        {
            Id = id;
            DisplayName = displayName;
            Summary = summary;
            DualRankBias = dualRankBias;
        }

        public StartingDeckId Id { get; }
        public string DisplayName { get; }
        public string Summary { get; }
        public bool DualRankBias { get; }

        public List<PlayingCardInstance> BuildDeck(Random random)
        {
            return Id switch
            {
                StartingDeckId.StyxCurrent => BuildStyxCurrent(random),
                StartingDeckId.Merchant => BuildMerchant(random),
                _ => PlayingCardFactory.CreateStandardDeck(dualRank: true, random: random)
            };
        }

        private static List<PlayingCardInstance> BuildStyxCurrent(Random random)
        {
            var deck = PlayingCardFactory.CreateStandardDeck(dualRank: true, random: random);
            for (var i = 0; i < deck.Count; i++)
            {
                if (deck[i].Suit == Suit.Hearts)
                {
                    deck[i] = PlayingCardFactory.Create(
                        deck[i].Suit,
                        deck[i].CornerA,
                        RankExtensions.ClampRank(deck[i].CornerA.ToSortValue() + random.Next(1, 4)));
                }
            }

            return deck;
        }

        private static List<PlayingCardInstance> BuildMerchant(Random random)
        {
            var deck = PlayingCardFactory.CreateStandardDeck(dualRank: true, random: random);
            for (var i = 0; i < deck.Count; i++)
            {
                if (deck[i].Suit == Suit.Diamonds && random.NextDouble() < 0.35d)
                {
                    var card = deck[i];
                    deck[i] = PlayingCardFactory.Create(card.Suit, card.CornerA, card.CornerB);
                }
            }

            deck.Add(PlayingCardFactory.Create(Suit.Diamonds, Rank.Ace, Rank.Seven, enchantment: EnchantmentType.FamineFold));
            return deck;
        }
    }

    public static class StartingDeckLibrary
    {
        public static StartingDeckDefinition Get(StartingDeckId id) =>
            id switch
            {
                StartingDeckId.StyxCurrent => StyxCurrent,
                StartingDeckId.Merchant => Merchant,
                _ => Standard
            };

        public static readonly StartingDeckDefinition Standard = new(
            StartingDeckId.Standard,
            "标准套牌",
            "52 张双面扑克，均衡分布",
            dualRankBias: true);

        public static readonly StartingDeckDefinition StyxCurrent = new(
            StartingDeckId.StyxCurrent,
            "冥河套牌",
            "红心牌双面差更大，适合 Pass 凑型",
            dualRankBias: true);

        public static readonly StartingDeckDefinition Merchant = new(
            StartingDeckId.Merchant,
            "商旅套牌",
            "多方块 · 含一张折角附魔",
            dualRankBias: true);
    }
}
