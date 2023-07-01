using GameData;
using Gear;
using Hikaria.WeaponDataLoader.Utils;
using Il2CppInterop.Runtime.Attributes;
using Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using JsonConverter = Hikaria.WeaponDataLoader.Utils.JsonConverter;

namespace Hikaria.WeaponDataLoader.Managers
{
    internal sealed class WeaponDataManager : MonoBehaviour
    {
        private void Awake()
        {
            Instance = this;

            LocalPlayer = PlayerManager.GetLocalPlayerAgent();

            switchFireModeKey = EntryPoint.Instance.SwitchFireModeKey.Value;
            reloadCustomDataKey = EntryPoint.Instance.ReloadCustomDataKey.Value;
            enableSwitchFireModeSound = EntryPoint.Instance.EnableSwitchFireModeSound.Value;
            switchFireModeSoundID = EntryPoint.Instance.SwitchFireModeSoundEventID.Value;

            foreach (var block in GameDataBlockBase<GearCategoryDataBlock>.GetAllBlocksForEditor())
            {
                OriginGearCategoryDataBlocks.Add(block.persistentID, block);
            }
            foreach (var block in GameDataBlockBase<ArchetypeDataBlock>.GetAllBlocksForEditor())
            {
                OriginArchetypeDataBlocks.Add(block.persistentID, block);
            }

            Instance.LoadCustomData();
        }

        private void Update()
        {
            if (Input.GetKeyDown(reloadCustomDataKey))
            {
                LoadCustomData();
            }
            if (GameStateManager.CurrentStateName != eGameStateName.InLevel || LocalPlayer.Locomotion.m_currentStateEnum == PlayerLocomotion.PLOC_State.Downed || wieldWeapon == null || PlayerChatManager.InChatMode)
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
            try
            {
                using (FileStream fs = File.Open(CUSTOM_CATEGORYDATA_PATH, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        JsonConverter converter = new JsonConverter();
                        string jsonContent = reader.ReadToEnd();
                        CustomGearCategoryDataBlocks.Clear();
                        CustomGearCategoryDataBlocks = converter.Deserialize<List<CustomGearCategoryDataBlock>>(jsonContent).ToDictionary(x => x.persistentID, x => x);
                        FireModeIndex.Clear();
                        foreach (uint key in CustomGearCategoryDataBlocks.Keys)
                        {
                            FireModeIndex.Add(key, 0);
                        }
                    }
                }
                GameEventLogManager.Current.AddLog("<color=orange>[WeaponDataManager]</color> <color=green>已读取 CustomGearCategoryDataBlocks</color>");
            }
            catch (Exception ex)
            {
                Logs.LogError(ex.Message);
                GameEventLogManager.Current.AddLog("<color=orange>[WeaponDataManager]</color> <color=red>读取 CustomGearCategoryDataBlocks 失败, 请查看 Log 获取更多信息!</color>");
            }

            try
            {
                using (FileStream fs = File.Open(CUSTOM_ARCHTYPEDATA_PATH, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        string jsonContent = reader.ReadToEnd();
                        JsonConverter converter = new JsonConverter();
                        List<CustomArchetypeDataBlock> blocks = converter.Deserialize<List<CustomArchetypeDataBlock>>(jsonContent);
                        CustomArchetypeDataBlocks.Clear();
                        foreach (CustomArchetypeDataBlock block in blocks)
                        {
                            CustomArchetypeDataBlocks.Add(block.persistentID, block);
                        }
                    }
                }
                GameEventLogManager.Current.AddLog("<color=orange>[WeaponDataManager]</color> <color=green>已读取 CustomArchetypeDataBlocks</color>");
            }
            catch (Exception ex)
            {
                Logs.LogError(ex.Message);
                GameEventLogManager.Current.AddLog("<color=orange>[WeaponDataManager]</color> <color=red>读取 CustomArchetypeDataBlocks 失败, 请查看 Log 获取更多信息!</color>");
            }

            try
            {
                using (FileStream fs = File.Open(CUSTOM_RECOILDATA_PATH, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        string jsonContent = reader.ReadToEnd();
                        JsonConverter converter = new JsonConverter();
                        CustomRecoilDataBlocks.Clear();
                        CustomRecoilDataBlocks = converter.Deserialize<List<CustomRecoilDataBlock>>(jsonContent).ToDictionary(x => x.persistentID, x => x);
                    }
                }
                GameEventLogManager.Current.AddLog("<color=orange>[WeaponDataManager]</color> <color=green>已读取 CustomRecoilDataBlocks</color>");
            }
            catch (Exception ex)
            {
                Logs.LogError(ex.Message);
                GameEventLogManager.Current.AddLog("<color=orange>[WeaponDataManager]</color> <color=red>读取 CustomRecoilDataBlocks 失败, 请查看 Log 获取更多信息!</color>");
            }
        }

        public bool TryGetNextFireMode(uint categoryPersistentID, eWeaponFireMode currentMode, out eWeaponFireMode nextMode, out string nextModeName, out CustomArchetypeDataBlock archtypeDataBlock, out CustomRecoilDataBlock recoilDataBlock)
        {
            nextMode = currentMode;
            archtypeDataBlock = null;
            recoilDataBlock = null;
            nextModeName = string.Empty;

            if (!CustomGearCategoryDataBlocks.TryGetValue(categoryPersistentID, out CustomGearCategoryDataBlock categoryDataBlock) || !categoryDataBlock.internalEnabled)
            {
                return false;
            }

            int preFireModeIndex = FireModeIndex[categoryPersistentID];
            FireModeIndex[categoryPersistentID] = (FireModeIndex[categoryPersistentID] + 1) > categoryDataBlock.FireModeSequence.Count - 1 ? 0 : FireModeIndex[categoryPersistentID] + 1;
            if (FireModeIndex[categoryPersistentID] == preFireModeIndex)
            {
                return false;
            }
            nextMode = categoryDataBlock.FireModeSequence[FireModeIndex[categoryPersistentID]];
            nextModeName = categoryDataBlock.FireModeNameSequence[FireModeIndex[categoryPersistentID]];

            if (!CustomArchetypeDataBlocks.TryGetValue(categoryDataBlock.ArchetypeDatas[FireModeIndex[categoryPersistentID]], out archtypeDataBlock))
            {
                return false;
            }
            if (!CustomRecoilDataBlocks.TryGetValue(archtypeDataBlock.RecoilDataID, out recoilDataBlock))
            {
                return false;
            }

            return true;
        }

        private void ApplyCustomData(CustomArchetypeDataBlock customArchetypeDataBlock, CustomRecoilDataBlock customRecoilDataBlock)
        {
            SavePrevAmmoPercent();
            ArchetypeDataBlock archetypeDataBlock = wieldWeapon.ArchetypeData;
            RecoilDataBlock recoilDataBlock = wieldWeapon.RecoilData;
            CopyBlock(customArchetypeDataBlock, ref archetypeDataBlock);
            CopyBlock(customRecoilDataBlock, ref recoilDataBlock);
            wieldWeapon.ArchetypeData = archetypeDataBlock;
            wieldWeapon.RecoilData = recoilDataBlock;
            wieldWeapon.SetupArchetype();
            wieldWeapon.m_archeType.m_recoilData = recoilDataBlock;
            HandlerLocalAmmoStorage();
            RestoreAmmoPercent();
            LocalPlayer.Sync.WantsToWieldSlot(wieldWeapon.m_inventory.WieldedSlot, false);
        }

        private void HandlerLocalAmmoStorage()
        {
            PlayerAmmoStorage ammoStorage = PlayerBackpackManager.LocalBackpack.AmmoStorage;
            InventorySlotAmmo inventorySlotAmmo = ammoStorage.GetInventorySlotAmmo(wieldWeapon.m_inventory.WieldedSlot);
            inventorySlotAmmo.Setup(wieldWeapon.ArchetypeData.CostOfBullet, wieldWeapon.ArchetypeData.DefaultClipSize, -1f);
        }
        private void SavePrevAmmoPercent()
        {
            float clipAmmo = wieldWeapon.m_clip;
            PlayerAmmoStorage ammoStorage = PlayerBackpackManager.LocalBackpack.AmmoStorage;
            InventorySlotAmmo inventorySlotAmmo = ammoStorage.GetInventorySlotAmmo(wieldWeapon.m_inventory.WieldedSlot);
            StoredAmmoBySlot[wieldWeapon.m_inventory.WieldedSlot] = (float)(clipAmmo + inventorySlotAmmo.BulletsInPack) / (float)inventorySlotAmmo.BulletsMaxCap;
        }

        private void RestoreAmmoPercent()
        {
            PlayerAmmoStorage ammoStorage = PlayerBackpackManager.LocalBackpack.AmmoStorage;
            InventorySlotAmmo inventorySlotAmmo = ammoStorage.GetInventorySlotAmmo(wieldWeapon.m_inventory.WieldedSlot);
            float num = (StoredAmmoBySlot[wieldWeapon.m_inventory.WieldedSlot] + (inventorySlotAmmo.IsFull ? 0f : 0.001f)) * (float)inventorySlotAmmo.BulletsMaxCap;
            inventorySlotAmmo.AmmoInPack = num * inventorySlotAmmo.CostOfBullet;
            ammoStorage.SetClipAmmoInSlot(wieldWeapon.m_inventory.WieldedSlot);
            ammoStorage.UpdateSlotAmmoUI(wieldWeapon.m_inventory.WieldedSlot);
            ammoStorage.NeedsSync = true;

        }

        public bool TryApplyCustomData(uint categoryPersistentID)
        {
            if (!CustomGearCategoryDataBlocks.TryGetValue(categoryPersistentID, out CustomGearCategoryDataBlock customCategoryDataBlock) || !customCategoryDataBlock.internalEnabled)
            {
                return false;
            }
            if (!CustomArchetypeDataBlocks.TryGetValue(customCategoryDataBlock.ArchetypeDatas[FireModeIndex[categoryPersistentID]], out CustomArchetypeDataBlock customArchetypeDataBlock) || !customArchetypeDataBlock.internalEnabled)
            {
                return false;
            }
            if (!CustomRecoilDataBlocks.TryGetValue(customArchetypeDataBlock.RecoilDataID, out CustomRecoilDataBlock customRecoilDataBlock) || !customRecoilDataBlock.internalEnabled)
            {
                return false;
            }

            ApplyCustomData(customArchetypeDataBlock, customRecoilDataBlock);
            return true;
        }

        public void SwitchFireMode()
        {
            if (TryGetNextFireMode(wieldWeapon.GearCategoryData.persistentID, wieldWeapon.ArchetypeData.FireMode, out eWeaponFireMode nextMode, out string nextModeName, out CustomArchetypeDataBlock archtypeDataBlock, out CustomRecoilDataBlock recoilDataBlock))
            {
                if (wieldWeapon.ArchetypeData.FireMode == eWeaponFireMode.Auto)
                {
                    wieldWeapon.TriggerAutoFireEndAudio();
                }
                ApplyCustomData(archtypeDataBlock, recoilDataBlock);
                GameEventLogManager.Current.AddLog(string.Format("<color=orange>[WeaponDataManager]</color> <color=green>已切换 {0} 开火模式为 {1}</color>", wieldWeapon.ArchetypeName, nextModeName));
            }
            else
            {
                GameEventLogManager.Current.AddLog(string.Format("<color=orange>[WeaponDataManager]</color> <color=red>{0} 不存在其他开火模式</color>", wieldWeapon.ArchetypeName));
            }
        }

        public void DoClear()
        {
            StoredAmmoBySlot.Clear();
        }

        private KeyCode switchFireModeKey = KeyCode.V;

        private KeyCode reloadCustomDataKey = KeyCode.F5;

        private uint switchFireModeSoundID = 0U;

        private bool enableSwitchFireModeSound = false;

        public static WeaponDataManager Instance;

        private PlayerAgent LocalPlayer;

        public BulletWeapon wieldWeapon;

        internal static readonly string CUSTOM_ARCHTYPEDATA_PATH = BepInEx.Paths.ConfigPath + "/Hikaria/WeaponDataManager/CustomArchetypeDataBlock.json";

        internal static readonly string CUSTOM_CATEGORYDATA_PATH = BepInEx.Paths.ConfigPath + "/Hikaria/WeaponDataManager/CustomGearCategoryDataBlock.json";

        internal static readonly string CUSTOM_RECOILDATA_PATH = BepInEx.Paths.ConfigPath + "/Hikaria/WeaponDataManager/CustomRecoilDataBlock.json";

        internal static readonly string CONFIG_PATH = BepInEx.Paths.ConfigPath + "/Hikaria/WeaponDataManager/Config.cfg";

        private Dictionary<uint, CustomArchetypeDataBlock> CustomArchetypeDataBlocks = new();

        private Dictionary<uint, CustomGearCategoryDataBlock> CustomGearCategoryDataBlocks = new();

        private Dictionary<uint, CustomRecoilDataBlock> CustomRecoilDataBlocks = new();

        public Dictionary<uint, ArchetypeDataBlock> OriginArchetypeDataBlocks = new();

        public Dictionary<uint, GearCategoryDataBlock> OriginGearCategoryDataBlocks = new();

        private static Dictionary<uint, int> FireModeIndex = new();

        public Dictionary<InventorySlot, float> StoredAmmoBySlot { get; private set; } = new();

        public class CustomGearCategoryDataBlock
        {
            public List<uint> ArchetypeDatas;

            public List<eWeaponFireMode> FireModeSequence;

            public List<string> FireModeNameSequence;

            public float OriginAmmoMax;

            public string name;

            public bool internalEnabled;

            public uint persistentID;
        }

        public class CustomRecoilDataBlock
        {
            public MinMaxValue power;

            public float spring;

            public float dampening;

            public float hipFireCrosshairSizeDefault;

            public float hipFireCrosshairRecoilPop;

            public float hipFireCrosshairSizeMax;

            public MinMaxValue horizontalScale;

            public MinMaxValue verticalScale;

            public float directionalSimilarity;

            public float worldToViewSpaceBlendHorizontal;

            public float worldToViewSpaceBlendVertical;

            public Vector3 recoilPosImpulse;

            public Vector3 recoilPosShift;

            public float recoilPosShiftWeight;

            public float recoilPosStiffness;

            public float recoilPosDamping;

            public float recoilPosImpulseWeight;

            public float recoilCameraPosWeight;

            public float recoilAimingWeight;

            public Vector3 recoilRotImpulse;

            public float recoilRotStiffness;

            public float recoilRotDamping;

            public float recoilRotImpulseWeight;

            public float recoilCameraRotWeight;

            public float concussionIntensity;

            public float concussionFrequency;

            public float concussionDuration;

            public string name;

            public bool internalEnabled;

            public uint persistentID;
        }

        public class CustomArchetypeDataBlock
        {
            public string PublicName;

            public string Description;

            public eWeaponFireMode FireMode;

            public uint RecoilDataID;

            public AgentModifier DamageBoosterEffect;

            public float Damage;

            public Vector2 DamageFalloff;

            public float StaggerDamageMulti;

            public float PrecisionDamageMulti;

            public int DefaultClipSize;

            public float DefaultReloadTime;

            public float CostOfBullet;

            public float ShotDelay;

            public float ShellCasingSize;

            public Vector2 ShellCasingSpeedRange;

            public bool PiercingBullets;

            public int PiercingDamageCountLimit;

            public float HipFireSpread;

            public float AimSpread;

            public float EquipTransitionTime;

            public List<WeaponAnimSequenceItem> EquipSequence;

            public float AimTransitionTime;

            public List<WeaponAnimSequenceItem> AimSequence;

            public float BurstDelay;

            public int BurstShotCount;

            public int ShotgunBulletCount;

            public int ShotgunConeSize;

            public int ShotgunBulletSpread;

            public float SpecialChargetupTime;

            public float SpecialCooldownTime;

            public float SpecialSemiBurstCountTimeout;

            public float Sentry_StartFireDelay;

            public float Sentry_RotationSpeed;

            public float Sentry_DetectionMaxRange;

            public float Sentry_DetectionMaxAngle;

            public bool Sentry_FireTowardsTargetInsteadOfForward;

            public float Sentry_LongRangeThreshold;

            public float Sentry_ShortRangeThreshold;

            public bool Sentry_LegacyEnemyDetection;

            public bool Sentry_FireTagOnly;

            public bool Sentry_PrioTag;

            public float Sentry_StartFireDelayTagMulti;

            public float Sentry_RotationSpeedTagMulti;

            public float Sentry_DamageTagMulti;

            public float Sentry_StaggerDamageTagMulti;

            public float Sentry_CostOfBulletTagMulti;

            public float Sentry_ShotDelayTagMulti;

            public string name;

            public bool internalEnabled;

            public uint persistentID;
        }

        public float GetOriginAmmoMaxCapForWieldWeapon(InventorySlot slot)
        {
            PlayerBackpackManager.LocalBackpack.TryGetBackpackItem(slot, out BackpackItem item);
            return CustomGearCategoryDataBlocks[item.Instance.Cast<BulletWeapon>().GearCategoryData.persistentID].OriginAmmoMax;
        }

        public void UpdateAmmoPercent(InventorySlot slot)
        {
            PlayerBackpackManager.LocalBackpack.TryGetBackpackItem(slot, out BackpackItem item);
            float clipAmmo = item.Instance.Cast<BulletWeapon>().m_clip;
            PlayerAmmoStorage ammoStorage = PlayerBackpackManager.LocalBackpack.AmmoStorage;
            InventorySlotAmmo inventorySlotAmmo = ammoStorage.GetInventorySlotAmmo(slot);
            StoredAmmoBySlot[slot] = (float)(clipAmmo + inventorySlotAmmo.BulletsInPack) / (float)inventorySlotAmmo.BulletsMaxCap;
        }

        private static void CopyBlock(CustomArchetypeDataBlock from, ref ArchetypeDataBlock to)
        {
            to.PublicName.Id = 0U;
            to.PublicName.OldId = 0U;
            to.PublicName.UntranslatedText = from.PublicName;

            to.Description.Id = 0U;
            to.Description.OldId = 0U;
            to.Description.UntranslatedText = from.Description;

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
            to.ShellCasingSize = from.ShellCasingSize;
            to.ShellCasingSpeedRange = from.ShellCasingSpeedRange;
            to.PiercingBullets = from.PiercingBullets;
            to.PiercingDamageCountLimit = from.PiercingDamageCountLimit;
            to.HipFireSpread = from.HipFireSpread;
            to.AimSpread = from.AimSpread;
            to.EquipTransitionTime = from.EquipTransitionTime;
            to.EquipSequence.Clear();
            foreach (var item in from.EquipSequence)
            {
                to.EquipSequence.Add(item);
            }
            to.AimTransitionTime = from.AimTransitionTime;
            to.AimSequence.Clear();
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
            to.name = from.name;
            to.internalEnabled = from.internalEnabled;
            to.persistentID = from.persistentID;
        }

        private static void CopyBlock(CustomRecoilDataBlock from, ref RecoilDataBlock to)
        {
            to.power = from.power;
            to.spring = from.spring;
            to.dampening = from.dampening;
            to.hipFireCrosshairSizeDefault = from.hipFireCrosshairSizeDefault;
            to.hipFireCrosshairRecoilPop = from.hipFireCrosshairRecoilPop;
            to.hipFireCrosshairSizeMax = from.hipFireCrosshairSizeMax;
            to.horizontalScale = from.horizontalScale;
            to.verticalScale = from.verticalScale;
            to.directionalSimilarity = from.directionalSimilarity;
            to.worldToViewSpaceBlendHorizontal = from.worldToViewSpaceBlendHorizontal;
            to.worldToViewSpaceBlendVertical = from.worldToViewSpaceBlendVertical;
            to.recoilPosImpulse = from.recoilPosImpulse;
            to.recoilPosShift = from.recoilPosShift;
            to.recoilPosShiftWeight = from.recoilPosShiftWeight;
            to.recoilPosStiffness = from.recoilPosStiffness;
            to.recoilPosDamping = from.recoilPosDamping;
            to.recoilPosImpulseWeight = from.recoilPosImpulseWeight;
            to.recoilCameraPosWeight = from.recoilCameraPosWeight;
            to.recoilAimingWeight = from.recoilAimingWeight;
            to.recoilRotImpulse = from.recoilRotImpulse;
            to.recoilRotStiffness = from.recoilRotStiffness;
            to.recoilRotDamping = from.recoilRotDamping;
            to.recoilRotImpulseWeight = from.recoilRotImpulseWeight;
            to.recoilCameraRotWeight = from.recoilCameraRotWeight;
            to.concussionIntensity = from.concussionIntensity;
            to.concussionFrequency = from.concussionFrequency;
            to.concussionDuration = from.concussionDuration;
            to.name = from.name;
            to.internalEnabled = from.internalEnabled;
            to.persistentID = from.persistentID;
        }
    }
}
