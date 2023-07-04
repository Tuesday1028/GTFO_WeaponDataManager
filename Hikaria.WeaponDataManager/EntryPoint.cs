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

            SwitchFireModeKey = configFile.Bind("按键设置", "SwitchFireModeKey", KeyCode.V, "开火模式切换按键");
            ReloadCustomDataKey = configFile.Bind("按键设置", "ReloadCustomDataKey", KeyCode.F5, "热重载数据按键");
            EnableSwitchFireModeSound = configFile.Bind("音效设置", "EnableSwitchFireModeSound", false, "启用开火模式切换音效");
            EnableHint = configFile.Bind("提示设置", "EnableHint", true, "启用开火模式切换提示信息");
            FireModeChangedHint = configFile.Bind("提示设置", "FireModeChangedHint", "{0} 当前开火模式: {1}", "开火模式切换提示信息, {0} 为枪械名称, {1} 为开火模式名称");
            FireModeChangedErrorHint = configFile.Bind("提示设置", "FireModeChangedErrorNotHaveHint", "{0} 存在配置错误", "开火模式切换错误提示信息, {0} 为枪械名称");

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

        internal ConfigEntry<string> FireModeChangedHint;

        internal ConfigEntry<string> FireModeChangedErrorHint;

        internal ConfigEntry<bool> EnableHint;

        private Harmony m_Harmony;

        internal static EntryPoint Instance;
    }
}
