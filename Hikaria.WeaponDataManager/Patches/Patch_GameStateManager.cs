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
    internal class Patch_GameStateManager
    {
        [HarmonyPatch(typeof(GameStateManager), nameof(GameStateManager.DoChangeState))]
        [HarmonyPostfix]
        private static void GameStateManager__DoChangeState__Postfix(eGameStateName nextState)
        {
            if (nextState == eGameStateName.InLevel)
            {
                WeaponDataManager.Instance.DoClear();
            }
        }
    }
}
