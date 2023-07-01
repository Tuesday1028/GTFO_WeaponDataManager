using GameData;
using Gear;
using HarmonyLib;
using Hikaria.WeaponDataLoader.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hikaria.WeaponDataLoader.Patches
{
    [HarmonyPatch]
    internal class Patch_Weapon
    {
        [HarmonyPatch(typeof(BulletWeapon), nameof(BulletWeapon.OnWield))]
        [HarmonyPostfix]
        private static void BulletWeapon__OnWield__Postfix(BulletWeapon __instance)
        {
            if (GameStateManager.CurrentStateName != eGameStateName.InLevel || !IsWeaponOwner(__instance) || __instance.ArchetypeData == null)
            {
                return;
            }
            WeaponDataLoadingManager.Instance.wieldWeapon = __instance;
            WeaponDataLoadingManager.Instance.TryApplyCustomData(__instance.GearCategoryData.persistentID);
        }

        [HarmonyPatch(typeof(BulletWeapon), nameof(BulletWeapon.OnUnWield))]
        [HarmonyPrefix]
        private static void BulletWeapon__OnWield__Prefix(BulletWeapon __instance)
        {
            if (GameStateManager.CurrentStateName != eGameStateName.InLevel || !IsWeaponOwner(__instance) || __instance.ArchetypeData == null)
            {
                return;
            }
            WeaponDataLoadingManager.Instance.wieldWeapon = null;
        }

        private static bool IsWeaponOwner(BulletWeapon weapon)
        {
            if (weapon.Owner == null)
            {
                return false;
            }
            return weapon.Owner.Owner.IsLocal;
        }
    }
}
