using NUnit.Framework;
using PasserCard.Encounter;
using PasserCard.Run;

namespace PasserCard.Tests
{
    public sealed class RunIdentityTests
    {
        [Test]
        public void SoulSplitter_AddsExtraPassToConfig()
        {
            var config = new EncounterConfig();
            RunIdentityLibrary.SoulSplitter.ApplyToConfig(config);
            Assert.AreEqual(1, config.IdentityExtraPasses);
        }

        [Test]
        public void CharonTribute_AppliesDiscount()
        {
            var full = CharonTributeSystem.GetActTribute(1, 0f);
            var discounted = CharonTributeSystem.GetActTribute(1, 0.1f);
            Assert.Less(discounted, full);
        }

        [Test]
        public void MerchantDeck_HasFamineCard()
        {
            var deck = StartingDeckLibrary.Merchant.BuildDeck(new System.Random(1));
            Assert.Greater(deck.Count, 52);
        }
    }
}
