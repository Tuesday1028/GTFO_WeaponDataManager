using GameData;
using Gear;
using Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using Hikaria.WeaponDataLoader.Data;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Hikaria.WeaponDataLoader.Utils;

namespace Hikaria.WeaponDataLoader.Managers
{
    internal sealed class WeaponDataManager : MonoBehaviour
    {
        private void Awake()
        {
            Instance = this;

            switchFireModeKey = EntryPoint.Instance.SwitchFireModeKey.Value;
            reloadCustomDataKey = EntryPoint.Instance.ReloadCustomDataKey.Value;
            enableSwitchFireModeSound = EntryPoint.Instance.EnableSwitchFireModeSound.Value;
            HINT_FIREMODECHANGED = EntryPoint.Instance.FireModeChangedHint.Value;
            HINT_FIREMODECHANGED_ERROR = EntryPoint.Instance.FireModeChangedErrorHint.Value;
            enableHint = EntryPoint.Instance.EnableHint.Value;

            CustomGearCategoryDataBlock.BaseSetup();
            CustomArchetypeDataBlock.BaseSetup();
            CustomRecoilDataBlock.BaseSetup();
            CustomWeaponAudioDataBlock.BaseSetup();

            LoadCustomData();
        }

        private void Update()
        {
            if (Input.GetKeyDown(reloadCustomDataKey))
            {
                LoadCustomData();
            }
            if (GameStateManager.CurrentStateName != eGameStateName.InLevel || LocalPlayer.Locomotion.m_currentStateEnum == PlayerLocomotion.PLOC_State.Downed || wieldWeapon == null || PlayerChatManager.InChatMode || wieldWeapon.m_archeType.Firing)
            {
                return;
            }
            if (Input.GetKeyDown(switchFireModeKey))
            {
                SwitchFireMode();
            }
        }

        private void LoadCustomData()
        {
            needHint = true;
            RegisteredWeapon.Clear();
            FireModeIndex.Clear();

            CustomGearCategoryDataBlock.DoLoadFromDisk();
            CustomArchetypeDataBlock.DoLoadFromDisk();
            CustomRecoilDataBlock.DoLoadFromDisk();
            CustomWeaponAudioDataBlock.DoLoadFromDisk();

            if (wieldWeapon != null)
            {
                TryApplyCustomData(wieldWeapon);
            }
        }

        public static bool TryGetNextFireMode(uint categoryPersistentID, eWeaponFireMode currentMode, out eWeaponFireMode nextMode, out string nextModeName, out CustomArchetypeDataBlock archtypeDataBlock, out CustomRecoilDataBlock recoilDataBlock, out CustomWeaponAudioDataBlock weaponAudioDataBlock)
        {
            nextMode = currentMode;
            archtypeDataBlock = null;
            recoilDataBlock = null;
            weaponAudioDataBlock = null;
            nextModeName = string.Empty;

            if (!CustomGearCategoryDataBlock.TryGetBlock(categoryPersistentID, out CustomGearCategoryDataBlock categoryDataBlock))
            {
                return false;
            }

            //int preFireModeIndex = FireModeIndex[categoryPersistentID];
            FireModeIndex[categoryPersistentID] = (FireModeIndex[categoryPersistentID] + 1) > categoryDataBlock.ArchetypeDataSequence.Count - 1 ? 0 : FireModeIndex[categoryPersistentID] + 1;
            /*
            if (FireModeIndex[categoryPersistentID] == preFireModeIndex)
            {
                return false;
            }
            */
            if (!CustomArchetypeDataBlock.TryGetBlock(categoryDataBlock.ArchetypeDataSequence[FireModeIndex[categoryPersistentID]], out archtypeDataBlock))
            {
                return false;
            }
            nextMode = archtypeDataBlock.FireMode;
            nextModeName = categoryDataBlock.FireModeNameSequence[FireModeIndex[categoryPersistentID]];

            if (!CustomRecoilDataBlock.TryGetBlock(archtypeDataBlock.RecoilDataID, out recoilDataBlock))
            {
                return false;
            }
            if (!CustomWeaponAudioDataBlock.TryGetBlock(categoryDataBlock.FireModeAudioSequence[FireModeIndex[categoryPersistentID]], out weaponAudioDataBlock))
            {
                return false;
            }

            return true;
        }

        private void ApplyCustomData(CustomArchetypeDataBlock customArchetypeDataBlock, CustomRecoilDataBlock customRecoilDataBlock, CustomWeaponAudioDataBlock customWeaponAudioDataBlock)
        {
            SavePrevAmmoPercent();
            ArchetypeDataBlock archetypeDataBlock = wieldWeapon.ArchetypeData;
            RecoilDataBlock recoilDataBlock = wieldWeapon.RecoilData;
            WeaponAudioDataBlock weaponAudioDataBlock = wieldWeapon.AudioData;
            CustomGameDataBlockBase<CustomArchetypeDataBlock>.CopyBlock(customArchetypeDataBlock, ref archetypeDataBlock);
            CustomGameDataBlockBase<CustomRecoilDataBlock>.CopyBlock(customRecoilDataBlock, ref recoilDataBlock);
            CustomGameDataBlockBase<CustomWeaponAudioDataBlock>.CopyBlock(customWeaponAudioDataBlock, ref weaponAudioDataBlock);
            wieldWeapon.ArchetypeData = archetypeDataBlock;
            wieldWeapon.RecoilData = recoilDataBlock;
            wieldWeapon.SetupArchetype();
            wieldWeapon.m_archeType.m_recoilData = recoilDataBlock;
            wieldWeapon.AudioData = weaponAudioDataBlock;
            wieldWeapon.SetupAudioEvents();
            SetupLocalAmmoStorage();
            RestoreAmmoPercent();
            LocalPlayer.Sync.WantsToWieldSlot(wieldWeapon.ItemDataBlock.inventorySlot, false);
        }

        private void SetupLocalAmmoStorage()
        {
            PlayerAmmoStorage ammoStorage = PlayerBackpackManager.LocalBackpack.AmmoStorage;
            InventorySlotAmmo inventorySlotAmmo = ammoStorage.GetInventorySlotAmmo(wieldWeapon.ItemDataBlock.inventorySlot);
            inventorySlotAmmo.Setup(wieldWeapon.ArchetypeData.CostOfBullet, wieldWeapon.ArchetypeData.DefaultClipSize, -1f);
        }
        private void SavePrevAmmoPercent()
        {
            PlayerAmmoStorage ammoStorage = PlayerBackpackManager.LocalBackpack.AmmoStorage;
            InventorySlotAmmo inventorySlotAmmo = ammoStorage.GetInventorySlotAmmo(wieldWeapon.ItemDataBlock.inventorySlot);
            StoredAmmoBySlot[wieldWeapon.ItemDataBlock.inventorySlot] = (wieldWeapon.m_clip * wieldWeapon.ArchetypeData.CostOfBullet + inventorySlotAmmo.AmmoInPack) / (inventorySlotAmmo.AmmoMaxCap + wieldWeapon.ArchetypeData.DefaultClipSize * wieldWeapon.ArchetypeData.CostOfBullet);
        }

        private void RestoreAmmoPercent()
        {
            PlayerAmmoStorage ammoStorage = PlayerBackpackManager.LocalBackpack.AmmoStorage;
            InventorySlotAmmo inventorySlotAmmo = ammoStorage.GetInventorySlotAmmo(wieldWeapon.ItemDataBlock.inventorySlot);

            if (wieldWeapon.m_clip > wieldWeapon.ArchetypeData.DefaultClipSize)
            {
                inventorySlotAmmo.AmmoInPack = StoredAmmoBySlot[wieldWeapon.ItemDataBlock.inventorySlot] * (inventorySlotAmmo.AmmoMaxCap + wieldWeapon.ArchetypeData.DefaultClipSize * wieldWeapon.ArchetypeData.CostOfBullet) - wieldWeapon.ArchetypeData.DefaultClipSize * wieldWeapon.ArchetypeData.CostOfBullet;
                wieldWeapon.m_clip = wieldWeapon.ArchetypeData.DefaultClipSize;
            }
            else
            {
                inventorySlotAmmo.AmmoInPack = StoredAmmoBySlot[wieldWeapon.ItemDataBlock.inventorySlot] * (inventorySlotAmmo.AmmoMaxCap + wieldWeapon.ArchetypeData.DefaultClipSize * wieldWeapon.ArchetypeData.CostOfBullet) - wieldWeapon.m_clip * wieldWeapon.ArchetypeData.CostOfBullet;
            }
            inventorySlotAmmo.AmmoInPack = Math.Min(Math.Max(0, inventorySlotAmmo.AmmoInPack), inventorySlotAmmo.AmmoMaxCap + wieldWeapon.ArchetypeData.DefaultClipSize * wieldWeapon.ArchetypeData.CostOfBullet);
            ammoStorage.UpdateSlotAmmoUI(inventorySlotAmmo.Slot);
            ammoStorage.NeedsSync = true;
        }

        public bool TryApplyCustomData(BulletWeapon weapon)
        {
            uint categoryID = weapon.GearCategoryData.persistentID;
            if (!CustomGearCategoryDataBlock.TryGetBlock(categoryID, out CustomGearCategoryDataBlock customCategoryDataBlock))
            {
                wieldWeapon = null;
                return false;
            }
            if (!FireModeIndex.ContainsKey(categoryID))
            {
                FireModeIndex.Add(categoryID, 0);
            }
            if (!CustomArchetypeDataBlock.TryGetBlock(customCategoryDataBlock.ArchetypeDataSequence[FireModeIndex[categoryID]], out CustomArchetypeDataBlock customArchetypeDataBlock))
            {
                wieldWeapon = null;
                return false;
            }
            if (!CustomRecoilDataBlock.TryGetBlock(customArchetypeDataBlock.RecoilDataID, out CustomRecoilDataBlock customRecoilDataBlock))
            {
                wieldWeapon = null;
                return false;
            }
            if (!CustomWeaponAudioDataBlock.TryGetBlock(customCategoryDataBlock.FireModeAudioSequence[FireModeIndex[categoryID]], out CustomWeaponAudioDataBlock customWeaponAudioDataBlock))
            {
                wieldWeapon = null;
                return false;
            }

            wieldWeapon = weapon;
            if (!RegisteredWeapon.Contains(categoryID))
            {
                needHint = true;
                RegisteredWeapon.Add(categoryID);
                if (!OriginArchetypeDataBlocksByCategoryID.ContainsKey(categoryID))
                {
                    ArchetypeDataBlock block = new();
                    CopyBlock(wieldWeapon.ArchetypeData, block);
                    OriginArchetypeDataBlocksByCategoryID.Add(categoryID, block);
                }
                ApplyCustomData(customArchetypeDataBlock, customRecoilDataBlock, customWeaponAudioDataBlock);
            }
            if (needHint && enableHint)
            {
                needHint = false;
                GameEventLogManager.Current.AddLog(string.Format(string.Format("<color=orange>[WeaponDataManager]</color> <color=green>{0}</color>", HINT_FIREMODECHANGED), wieldWeapon.ArchetypeName, CustomGearCategoryDataBlock.GetBlock(categoryID).FireModeNameSequence[FireModeIndex[categoryID]]));
            }

            return true;
        }

        public void SwitchFireMode()
        {
            if (TryGetNextFireMode(wieldWeapon.GearCategoryData.persistentID, wieldWeapon.ArchetypeData.FireMode, out eWeaponFireMode nextMode, out string nextModeName, out CustomArchetypeDataBlock archtypeDataBlock, out CustomRecoilDataBlock recoilDataBlock, out CustomWeaponAudioDataBlock weaponAudioDataBlock))
            {
                ApplyCustomData(archtypeDataBlock, recoilDataBlock, weaponAudioDataBlock);
                if (enableSwitchFireModeSound)
                {
                    wieldWeapon.Sound.Post(CustomGearCategoryDataBlock.GetBlock(wieldWeapon.GearCategoryData.persistentID).SwitchFireModeAudioEventID, wieldWeapon.transform.position);
                }
                if (enableHint)
                {
                    GameEventLogManager.Current.AddLog(string.Format(string.Format("<color=orange>[WeaponDataManager]</color> <color=green>{0}</color>", HINT_FIREMODECHANGED), wieldWeapon.ArchetypeName, nextModeName));
                }
            }
            else
            {
                GameEventLogManager.Current.AddLog(string.Format(string.Format("<color=orange>[WeaponDataManager]</color> <color=red>{0}</color>", HINT_FIREMODECHANGED_ERROR), wieldWeapon.ArchetypeName));
            }
        }

        public void DoClear()
        {
            needHint = true;
            StoredAmmoBySlot.Clear();
            RegisteredWeapon.Clear();
            FireModeIndex.Clear();
            ResetLocalAmmoStorage();
        }

        private static void ResetLocalAmmoStorage()
        {
            BackpackItem item;
            BulletWeapon weapon;
            ArchetypeDataBlock block;

            PlayerBackpackManager.LocalBackpack.TryGetBackpackItem(InventorySlot.GearStandard, out item);
            weapon = item.Instance.Cast<BulletWeapon>();
            if (OriginArchetypeDataBlocksByCategoryID.TryGetValue(weapon.GearCategoryData.persistentID, out block))
            {
                ArchetypeDataBlock to = new();
                CopyBlock(block, to);
                weapon.ArchetypeData = to;
                PlayerBackpackManager.LocalBackpack.SetupAmmoStorageForItem(weapon, InventorySlot.GearStandard);
            }

            PlayerBackpackManager.LocalBackpack.TryGetBackpackItem(InventorySlot.GearSpecial, out item);
            weapon = item.Instance.Cast<BulletWeapon>();
            if (OriginArchetypeDataBlocksByCategoryID.TryGetValue(weapon.GearCategoryData.persistentID, out block))
            {
                ArchetypeDataBlock to = new();
                CopyBlock(block, to);
                weapon.ArchetypeData = to;
                PlayerBackpackManager.LocalBackpack.SetupAmmoStorageForItem(weapon, InventorySlot.GearSpecial);
            }
        }

        private KeyCode switchFireModeKey = KeyCode.V;

        private KeyCode reloadCustomDataKey = KeyCode.F5;

        private bool enableSwitchFireModeSound = false;

        public static WeaponDataManager Instance;

        private PlayerAgent LocalPlayer { get { return PlayerManager.GetLocalPlayerAgent(); } }

        public BulletWeapon wieldWeapon;

        private bool needHint;

        private bool enableHint;

        private static string HINT_FIREMODECHANGED = "{0} 当前开火模式: {1}";

        private static string HINT_FIREMODECHANGED_ERROR = "{0} 存在配置错误";

        internal static readonly string CUSTOM_ARCHETYPEDATA_PATH = BepInEx.Paths.ConfigPath + "/Hikaria/WeaponDataManager/CustomArchetypeDataBlock.json";

        internal static readonly string CUSTOM_GEARCATEGORYDATA_PATH = BepInEx.Paths.ConfigPath + "/Hikaria/WeaponDataManager/CustomGearCategoryDataBlock.json";

        internal static readonly string CUSTOM_RECOILDATA_PATH = BepInEx.Paths.ConfigPath + "/Hikaria/WeaponDataManager/CustomRecoilDataBlock.json";

        internal static readonly string CUSTOM_WEAPONAUDIODATA_PATH = BepInEx.Paths.ConfigPath + "/Hikaria/WeaponDataManager/CustomWeaponAudioDataBlock.json";

        internal static readonly string CONFIG_PATH = BepInEx.Paths.ConfigPath + "/Hikaria/WeaponDataManager/Config.cfg";

        private static Dictionary<uint, int> FireModeIndex = new();

        private static Dictionary<uint, ArchetypeDataBlock> OriginArchetypeDataBlocksByCategoryID = new();

        private static List<uint> RegisteredWeapon = new();

        public Dictionary<InventorySlot, float> StoredAmmoBySlot { get; private set; } = new();

        public void UpdateAmmoPercent(InventorySlot slot)
        {
            PlayerBackpackManager.LocalBackpack.TryGetBackpackItem(slot, out BackpackItem item);
            BulletWeapon weapon = item.Instance.Cast<BulletWeapon>();
            PlayerAmmoStorage ammoStorage = PlayerBackpackManager.LocalBackpack.AmmoStorage;
            InventorySlotAmmo inventorySlotAmmo = ammoStorage.GetInventorySlotAmmo(slot);
            StoredAmmoBySlot[slot] = (weapon.m_clip * weapon.ArchetypeData.CostOfBullet + inventorySlotAmmo.AmmoInPack) / (inventorySlotAmmo.AmmoMaxCap + weapon.ArchetypeData.DefaultClipSize * inventorySlotAmmo.CostOfBullet);
        }

        public float GetOriginAmmoForSlot(InventorySlot slot)
        {
            PlayerBackpackManager.LocalBackpack.TryGetBackpackItem(slot, out BackpackItem item);
            BulletWeapon weapon = item.Instance.Cast<BulletWeapon>();
            return StoredAmmoBySlot[slot] * CustomGearCategoryDataBlock.GetBlock(weapon.GearCategoryData.persistentID).OriginAmmoMax;
        }

        public static void CopyBlock(ArchetypeDataBlock from, ArchetypeDataBlock to)
        {
            to.PublicName = from.PublicName;
            to.Description = from.Description;
            to.FireMode = from.FireMode;
            to.RecoilDataID = from.RecoilDataID;
            to.DamageBoosterEffect = from.DamageBoosterEffect;
            to.Damage = from.Damage;
            to.DamageFalloff = from.DamageFalloff;
            to.StaggerDamageMulti = from.StaggerDamageMulti;
            to.PrecisionDamageMulti = from.PrecisionDamageMulti;
            to.DefaultClipSize = from.DefaultClipSize;
            to.DefaultReloadTime = from.DefaultReloadTime;
            to.CostOfBullet = from.CostOfBullet;
            to.ShotDelay = from.ShotDelay;
            to.PiercingBullets = from.PiercingBullets;
            to.PiercingDamageCountLimit = from.PiercingDamageCountLimit;
            to.HipFireSpread = from.HipFireSpread;
            to.AimSpread = from.AimSpread;
            to.EquipTransitionTime = from.EquipTransitionTime;
            to.EquipSequence = new Il2CppSystem.Collections.Generic.List<WeaponAnimSequenceItem>();
            foreach (var item in from.EquipSequence)
            {
                to.EquipSequence.Add(item);
            }
            to.AimTransitionTime = from.AimTransitionTime;
            to.AimSequence = new Il2CppSystem.Collections.Generic.List<WeaponAnimSequenceItem>();
            foreach (var item in from.AimSequence)
            {
                to.AimSequence.Add(item);
            }
            to.BurstDelay = from.BurstDelay;
            to.BurstShotCount = from.BurstShotCount;
            to.ShotgunBulletCount = from.ShotgunBulletCount;
            to.ShotgunConeSize = from.ShotgunConeSize;
            to.ShotgunBulletSpread = from.ShotgunBulletSpread;
            to.SpecialChargetupTime = from.SpecialChargetupTime;
            to.SpecialCooldownTime = from.SpecialCooldownTime;
            to.SpecialSemiBurstCountTimeout = from.SpecialSemiBurstCountTimeout;
            to.Sentry_StartFireDelay = from.Sentry_StartFireDelay;
            to.Sentry_RotationSpeed = from.Sentry_RotationSpeed;
            to.Sentry_DetectionMaxRange = from.Sentry_DetectionMaxRange;
            to.Sentry_DetectionMaxAngle = from.Sentry_DetectionMaxAngle;
            to.Sentry_FireTowardsTargetInsteadOfForward = from.Sentry_FireTowardsTargetInsteadOfForward;
            to.Sentry_LongRangeThreshold = from.Sentry_LongRangeThreshold;
            to.Sentry_ShortRangeThreshold = from.Sentry_ShortRangeThreshold;
            to.Sentry_LegacyEnemyDetection = from.Sentry_LegacyEnemyDetection;
            to.Sentry_FireTagOnly = from.Sentry_FireTagOnly;
            to.Sentry_PrioTag = from.Sentry_PrioTag;
            to.Sentry_StartFireDelayTagMulti = from.Sentry_StartFireDelayTagMulti;
            to.Sentry_RotationSpeedTagMulti = from.Sentry_RotationSpeedTagMulti;
            to.Sentry_DamageTagMulti = from.Sentry_DamageTagMulti;
            to.Sentry_StaggerDamageTagMulti = from.Sentry_StaggerDamageTagMulti;
            to.Sentry_CostOfBulletTagMulti = from.Sentry_CostOfBulletTagMulti;
            to.Sentry_ShotDelayTagMulti = from.Sentry_ShotDelayTagMulti;
        }
    }
}
