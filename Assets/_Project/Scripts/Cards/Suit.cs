namespace PasserCard.Cards
{
    public enum Suit
    {
        Spades = 0,
        Hearts = 1,
        Clubs = 2,
        Diamonds = 3
    }

    public enum PlagueSuit
    {
        None = 0,
        CorruptSpring = 1,
        FesterBlood = 2,
        WitherBlight = 3,
        RustRot = 4
    }

    public static class SuitExtensions
    {
        public static PlagueSuit ToPlagueVariant(this Suit suit) =>
            suit switch
            {
                Suit.Spades => PlagueSuit.CorruptSpring,
                Suit.Hearts => PlagueSuit.FesterBlood,
                Suit.Clubs => PlagueSuit.WitherBlight,
                Suit.Diamonds => PlagueSuit.RustRot,
                _ => PlagueSuit.None
            };

        public static Suit FromPlagueVariant(PlagueSuit plague) =>
            plague switch
            {
                PlagueSuit.CorruptSpring => Suit.Spades,
                PlagueSuit.FesterBlood => Suit.Hearts,
                PlagueSuit.WitherBlight => Suit.Clubs,
                PlagueSuit.RustRot => Suit.Diamonds,
                _ => Suit.Spades
            };
    }
}
