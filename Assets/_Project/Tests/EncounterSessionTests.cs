using System;
using NUnit.Framework;
using PasserCard.Cards;
using PasserCard.Encounter;
using PasserCard.Run;

namespace PasserCard.Tests
{
    public sealed class EncounterSessionTests
    {
        [Test]
        public void MistTable_FogSlot_RejectsPassedCard()
        {
            var config = new EncounterConfig { HandSize = 5, MistFogSlots = 2, PlayQuotas = 1 };
            var deck = PlayingCardFactory.CreateStandardDeck();
            var wallet = new SoulCoinWallet(10);
            var session = new EncounterSession(config, deck, wallet, new Random(1));
            session.StartEncounter();

            session.EnterPassPhase();
            var card = session.Hand.Cards[0];
            Assert.IsTrue(session.TryPassCard(card.InstanceId).Success);

            session.EnterPlayPhase();
            var fogSlotIndex = -1;
            for (var i = 0; i < session.PlayArea.Slots.Count; i++)
            {
                if (session.PlayArea.Slots[i].IsFog)
                {
                    fogSlotIndex = i;
                    break;
                }
            }

            Assert.GreaterOrEqual(fogSlotIndex, 0);
            Assert.IsFalse(session.TryPlaceCard(fogSlotIndex, card.InstanceId));
        }

        [Test]
        public void PinToFogSlot_LocksCardBeforePass()
        {
            var config = new EncounterConfig { HandSize = 5, MistFogSlots = 2, PlayQuotas = 1 };
            var deck = PlayingCardFactory.CreateStandardDeck();
            var wallet = new SoulCoinWallet(10);
            var session = new EncounterSession(config, deck, wallet, new Random(2));
            session.StartEncounter();

            session.EnterPrePinPhase();
            var card = session.Hand.Cards[0];
            var fogSlotIndex = 0;
            for (var i = 0; i < session.PlayArea.Slots.Count; i++)
            {
                if (session.PlayArea.Slots[i].IsFog)
                {
                    fogSlotIndex = i;
                    break;
                }
            }

            Assert.IsTrue(session.TryPinToFogSlot(fogSlotIndex, card.InstanceId));
            session.EnterPassPhase();
            Assert.IsFalse(session.TryPassCard(card.InstanceId).Success);
        }
    }
}
