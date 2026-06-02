using System;

namespace PasserCard.Run
{
    public static class CharonTributeSystem
    {
        public static int GetActTribute(int act, float charonDiscount = 0f)
        {
            var baseCost = act switch
            {
                1 => 8,
                2 => 14,
                3 => 22,
                _ => 30
            };

            var discounted = (int)Math.Round(baseCost * (1f - Math.Clamp(charonDiscount, 0f, 0.5f)));
            return Math.Max(1, discounted);
        }

        public static bool TryPayTribute(SoulCoinWallet wallet, int act, float charonDiscount, out int paid)
        {
            paid = GetActTribute(act, charonDiscount);
            return wallet.TrySpend(paid);
        }
    }
}
