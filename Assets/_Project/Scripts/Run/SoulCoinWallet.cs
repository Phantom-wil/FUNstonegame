namespace PasserCard.Run
{
    public sealed class SoulCoinWallet
    {
        public SoulCoinWallet(int startingCoins = 0)
        {
            Balance = startingCoins;
        }

        public int Balance { get; private set; }

        public void Add(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Balance += amount;
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            if (Balance < amount)
            {
                return false;
            }

            Balance -= amount;
            return true;
        }
    }
}
