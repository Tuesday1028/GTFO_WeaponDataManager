using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using GameData;
using HarmonyLib;
using Hikaria.WeaponDataLoader.Managers;
using Hikaria.WeaponDataLoader.Utils;
using Il2CppInterop.Runtime.Injection;
using System.IO;
using UnityEngine;

namespace Hikaria.WeaponDataLoader
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class EntryPoint : BasePlugin
    {
        public override void Load()
        {
            Instance = this;

            SwitchFireModeKey = configFile.Bind("按键设置", "SwitchFireModeKey", KeyCode.V, "切换开火模式按键");
            ReloadCustomDataKey = configFile.Bind("按键设置", "ReloadCustomDataKey", KeyCode.F5, "热重载数据按键");
            EnableSwitchFireModeSound = configFile.Bind("音效设置", "EnableSwitchFireModeSound", false, "启用开火模式音效");
            SwitchFireModeSoundEventID = configFile.Bind("音效设置", "SwitchFireModeSoundEventID", 0U, "切换开火模式音效ID");

            ClassInjector.RegisterTypeInIl2Cpp<GameEventLogManager>();
            ClassInjector.RegisterTypeInIl2Cpp<WeaponDataManager>();

            m_Harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            m_Harmony.PatchAll();

            Logs.LogMessage("OK");
        }

        private static ConfigFile configFile = new ConfigFile(WeaponDataManager.CONFIG_PATH, true);

        internal ConfigEntry<KeyCode> SwitchFireModeKey;

        internal ConfigEntry<KeyCode> ReloadCustomDataKey;

        internal ConfigEntry<bool> EnableSwitchFireModeSound;

        internal ConfigEntry<uint> SwitchFireModeSoundEventID;

        private Harmony m_Harmony;

        internal static EntryPoint Instance;
    }
}
