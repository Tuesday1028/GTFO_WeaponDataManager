using HarmonyLib;
using Hikaria.WeaponDataLoader.Managers;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hikaria.WeaponDataLoader.Patches
{
    [HarmonyPatch]
    internal class Patch_LocalPlayerAgent
    {
        [HarmonyPatch(typeof(LocalPlayerAgent), nameof(LocalPlayerAgent.Setup))]
        [HarmonyPostfix]
        private static void LocalPlayerAgent__Setup__Postfix(LocalPlayerAgent __instance)
        {
            GameObject gameObject = GameObject.Find("WeaponDataManager_Handlers");
            if (gameObject == null)
            {
                gameObject = new GameObject("WeaponDataManager_Handlers");
            }            
            if (gameObject.GetComponent<GameEventLogManager>() == null)
            {
                gameObject.AddComponent<GameEventLogManager>();
            }
            if (gameObject.GetComponent<WeaponDataManager>() == null)
            {
                gameObject.AddComponent<WeaponDataManager>();
            }
        }
    }
}
