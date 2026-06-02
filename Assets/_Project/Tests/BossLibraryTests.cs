using NUnit.Framework;
using PasserCard.Boss;
using PasserCard.Encounter;

namespace PasserCard.Tests
{
    public sealed class BossLibraryTests
    {
        [Test]
        public void Nyx_HasThreeFogSlotsAndHideCorners()
        {
            var config = BossLibrary.Nyx.ToEncounterConfig();
            Assert.AreEqual(3, config.MistFogSlots);
            Assert.IsTrue(config.HideFogCorners);
            Assert.AreEqual(BossId.Nyx, config.BossId);
        }

        [Test]
        public void Styx_UsesSmoothIceTable()
        {
            var config = BossLibrary.Styx.ToEncounterConfig();
            Assert.AreEqual(Table.TableEnvironmentId.SmoothIce, config.TableEnvironment);
        }

        [Test]
        public void Cerberus_LimitsPasses()
        {
            var config = BossLibrary.Cerberus.ToEncounterConfig();
            Assert.AreEqual(3, config.PassLimit);
        }
    }
}
