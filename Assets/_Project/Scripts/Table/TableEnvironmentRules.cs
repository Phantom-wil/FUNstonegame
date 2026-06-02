using System;
using PasserCard.Cards;
using PasserCard.Encounter;
using PasserCard.Enchantments;
using PasserCard.Pass;
using PasserCard.Run;

namespace PasserCard.Table
{
    public static class TableEnvironmentRules
    {
        public static PassResult ApplyPostPassTableEffect(
            PlayingCardInstance card,
            PassResult result,
            EncounterConfig config,
            PassState passState,
            Random random)
        {
            if (!result.Success || config.TableEnvironment != TableEnvironmentId.SmoothIce)
            {
                return result;
            }

            if (random.NextDouble() > 0.25d || card.IsPinnedToFogSlot)
            {
                return result;
            }

            card.TogglePassFace();
            return new PassResult(true, result.Message + " 光滑牌桌：额外翻转一次。", result.ConsumedPassToken);
        }

        public static bool TryPassThornTable(PlayingCardInstance card, EncounterConfig config, SoulCoinWallet wallet)
        {
            if (config.TableEnvironment != TableEnvironmentId.ThornPath)
            {
                return true;
            }

            if (card.WasPassedThisQuota)
            {
                return true;
            }

            return wallet.TrySpend(1);
        }

        public static int ThornUnpassedPlayCost(PlayingCardInstance card, EncounterConfig config)
        {
            if (config.TableEnvironment != TableEnvironmentId.ThornPath)
            {
                return 0;
            }

            return card.WasPassedThisQuota ? 0 : 1;
        }

        public static float RiftCrackSplitBonus(EncounterConfig config, PlayingCardInstance card)
        {
            if (config.TableEnvironment != TableEnvironmentId.RiftDepth)
            {
                return 1f;
            }

            return card.Enchantment == EnchantmentType.WarCrack ? 1.5f : 1f;
        }
    }
}
