using HarmonyLib;
using Player;
using Hikaria.WeaponDataLoader.Managers;
using GameData;

namespace Hikaria.WeaponDataLoader.Patches
{
    [HarmonyPatch]
    internal class Patch_PlayerAmmoStorage
    {
        [HarmonyPatch(typeof(PlayerAmmoStorage), nameof(PlayerAmmoStorage.GetStorageData))]
        [HarmonyPostfix]
        private static void PlayerAmmoStorage__GetStorageData__Postfix(PlayerAmmoStorage __instance, ref pAmmoStorageData __result)
        {
            if (!__instance.m_playerBackpack.Owner.IsLocal || GameStateManager.CurrentStateName != eGameStateName.InLevel)
            {
                return;
            }

            if (WeaponDataManager.Instance.StoredAmmoBySlot.ContainsKey(InventorySlot.GearStandard))
            {
                WeaponDataManager.Instance.UpdateAmmoPercent(InventorySlot.GearStandard);
                __result.standardAmmo.Set(WeaponDataManager.Instance.GetOriginAmmoForSlot(InventorySlot.GearStandard), 500f);
            }
            if (WeaponDataManager.Instance.StoredAmmoBySlot.ContainsKey(InventorySlot.GearSpecial))
            {
                WeaponDataManager.Instance.UpdateAmmoPercent(InventorySlot.GearSpecial);
                __result.specialAmmo.Set(WeaponDataManager.Instance.GetOriginAmmoForSlot(InventorySlot.GearSpecial), 500f);
            }
        }
    }
}
