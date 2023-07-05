using GameData;
using Player;
using System;
using System.Collections.Generic;
using Hikaria.WeaponDataLoader.Utils;
using UnityEngine;
using Hikaria.WeaponDataLoader.Managers;
using System.Linq;
using System.IO;
using System.Reflection;

namespace Hikaria.WeaponDataLoader.Data
{
    [Serializable]
    internal class CustomGameDataBlockWrapper<T> where T : CustomGameDataBlockBase<T>
    {
        public List<GameDataBlockListHeader> Headers { get; set; }

        public List<T> Blocks { get; set; }

        public uint LastPersistentID { get; set; }
    }

    [Serializable]
    internal class CustomGameDataBlockBase<T> where T : CustomGameDataBlockBase<T>
    {
        public string name { get; set; }

        public bool internalEnabled { get; set; }

        public uint persistentID { get; set; }

        public static CustomGameDataBlockWrapper<T> Wrapper;

        protected static Dictionary<uint, T> s_blockByID;

        public static void BaseSetup()
        {
            Wrapper = new();
            s_blockByID = new();
        }

        public static bool TryGetBlock(uint persistentID, out T block)
        {
            block = default;
            if (persistentID == 0U)
            {
                return false;
            }
            if (s_blockByID.TryGetValue(persistentID, out block))
            {
                return block.internalEnabled;
            }
            return false;
        }

        public static T GetBlock(uint persistentID)
        {
            return s_blockByID[persistentID];
        }

        public static T[] GetAllBlocksForEditor()
        {
            return Wrapper.Blocks.ToArray();
        }

        public static bool HasBlock(uint ID)
        {
            return s_blockByID != null && s_blockByID.ContainsKey(ID) && s_blockByID[ID].internalEnabled;
        }

        public static void CopyBlock(CustomArchetypeDataBlock from, ref ArchetypeDataBlock to)
        {
            to.PublicName.Id = from.PublicName;
            to.PublicName.OldId = from.PublicName;
            to.Description.Id = from.Description;
            to.Description.OldId = from.Description;
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

        public static void CopyBlock(CustomRecoilDataBlock from, ref RecoilDataBlock to)
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

        public static void CopyBlock(CustomWeaponAudioDataBlock from, ref WeaponAudioDataBlock to)
        {
            to.eventOnSemiFire2D = from.eventOnSemiFire2D.ToIl2CppList();
            to.eventOnBurstFire2D = from.eventOnBurstFire2D.ToIl2CppList();
            to.eventOnAutoFireStart2D = from.eventOnAutoFireStart2D.ToIl2CppList();
            to.eventOnAutoFireEnd2D = from.eventOnAutoFireEnd2D.ToIl2CppList();
            to.eventOnBurstFireOneShot2D = from.eventOnBurstFireOneShot2D.ToIl2CppList();
            to.eventOnChargeup2D = from.eventOnChargeup2D.ToIl2CppList();
            to.eventOnCooldown2D = from.eventOnCooldown2D.ToIl2CppList();
            to.eventOnChargeupEnd2D = from.eventOnChargeupEnd2D;
            to.eventOnCooldownEnd2D = from.eventOnCooldownEnd2D;
            to.eventOnSemiFire3D = from.eventOnSemiFire3D.ToIl2CppList();
            to.eventOnBurstFire3D = from.eventOnBurstFire3D.ToIl2CppList();
            to.eventOnAutoFireStart3D = from.eventOnAutoFireStart3D.ToIl2CppList();
            to.eventOnAutoFireEnd3D = from.eventOnAutoFireEnd3D.ToIl2CppList();
            to.eventOnChargeup3D = from.eventOnChargeup3D.ToIl2CppList();
            to.eventOnCooldown3D = from.eventOnCooldown3D.ToIl2CppList();
            to.eventOnChargeupEnd3D = from.eventOnChargeupEnd3D;
            to.eventOnCooldownEnd3D = from.eventOnCooldownEnd3D;
            to.eventOnSyncedBurstFirePerShot3D = from.eventOnSyncedBurstFirePerShot3D.ToIl2CppList();
            to.eventOnSyncedAutoFirePerShot3D = from.eventOnSyncedAutoFirePerShot3D.ToIl2CppList();
            to.eventClick = from.eventClick;
            to.eventReload = from.eventReload;
            to.TriggerBurstAudioForEachShot = from.TriggerBurstAudioForEachShot;
            to.TriggerAutoAudioForEachShot = from.TriggerAutoAudioForEachShot;
            to.eventEquip = from.eventEquip;
            to.eventZoomIn = from.eventZoomIn;
            to.eventZoomOut = from.eventZoomOut;
            to.name = from.name;
            to.internalEnabled = from.internalEnabled;
            to.persistentID = from.persistentID;
        }
    }

    [Serializable]
    internal class CustomGearCategoryDataBlock : CustomGameDataBlockBase<CustomGearCategoryDataBlock>
    {
        public List<uint> ArchetypeDataSequence { get; set; }

        public List<string> FireModeNameSequence { get; set; }

        public List<uint> FireModeAudioSequence { get; set; }

        public uint SwitchFireModeAudioEventID { get; set; }

        public float OriginAmmoMax { get; set; }

        public static void DoLoadFromDisk()
        {
            try
            {
                using (FileStream fs = File.Open(WeaponDataManager.CUSTOM_GEARCATEGORYDATA_PATH, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        string jsonContent = reader.ReadToEnd();
                        JsonConverter converter = new();
                        Wrapper = converter.Deserialize<CustomGameDataBlockWrapper<CustomGearCategoryDataBlock>>(jsonContent);
                        s_blockByID.Clear();
                        s_blockByID = Wrapper.Blocks.ToDictionary(block => block.persistentID, block => block);
                    }
                }
                GameEventLogManager.Current.AddLog("<color=orange>[WeaponDataManager]</color> <color=green>已读取 CustomGearCategoryDataBlocks</color>");
            }
            catch (Exception ex)
            {
                Logs.LogError(ex.Message);
                GameEventLogManager.Current.AddLog("<color=orange>[WeaponDataManager]</color> <color=red>读取 CustomGearCategoryDataBlocks 失败, 请查看 Log 获取更多信息!</color>");
            }
        }
    }

    [Serializable]
    internal class CustomArchetypeDataBlock : CustomGameDataBlockBase<CustomArchetypeDataBlock>
    {
        public uint PublicName { get; set; }

        public uint Description { get; set; }

        public eWeaponFireMode FireMode { get; set; }

        public uint RecoilDataID { get; set; }

        public AgentModifier DamageBoosterEffect { get; set; }

        public float Damage { get; set; }

        public Vector2 DamageFalloff { get; set; }

        public float StaggerDamageMulti { get; set; }

        public float PrecisionDamageMulti { get; set; }

        public int DefaultClipSize { get; set; }

        public float DefaultReloadTime { get; set; }

        public float CostOfBullet { get; set; }

        public float ShotDelay { get; set; }

        public float ShellCasingSize { get; set; }

        public Vector2 ShellCasingSpeedRange { get; set; }

        public bool PiercingBullets { get; set; }

        public int PiercingDamageCountLimit { get; set; }

        public float HipFireSpread { get; set; }

        public float AimSpread { get; set; }

        public float EquipTransitionTime { get; set; }

        public List<WeaponAnimSequenceItem> EquipSequence { get; set; }

        public float AimTransitionTime { get; set; }

        public List<WeaponAnimSequenceItem> AimSequence { get; set; }

        public float BurstDelay { get; set; }

        public int BurstShotCount { get; set; }

        public int ShotgunBulletCount { get; set; }

        public int ShotgunConeSize { get; set; }

        public int ShotgunBulletSpread { get; set; }

        public float SpecialChargetupTime { get; set; }

        public float SpecialCooldownTime { get; set; }

        public float SpecialSemiBurstCountTimeout { get; set; }

        public float Sentry_StartFireDelay { get; set; }

        public float Sentry_RotationSpeed { get; set; }
        public float Sentry_DetectionMaxRange { get; set; }

        public float Sentry_DetectionMaxAngle { get; set; }

        public bool Sentry_FireTowardsTargetInsteadOfForwar { get; set; }

        public float Sentry_LongRangeThreshold { get; set; }

        public float Sentry_ShortRangeThreshold { get; set; }

        public bool Sentry_LegacyEnemyDetection { get; set; }

        public bool Sentry_FireTagOnly { get; set; }

        public bool Sentry_FireTowardsTargetInsteadOfForward { get; set; }

        public bool Sentry_PrioTag { get; set; }

        public float Sentry_StartFireDelayTagMulti { get; set; }

        public float Sentry_RotationSpeedTagMulti { get; set; }

        public float Sentry_DamageTagMulti { get; set; }

        public float Sentry_StaggerDamageTagMulti { get; set; }

        public float Sentry_CostOfBulletTagMulti { get; set; }

        public float Sentry_ShotDelayTagMulti { get; set; }

        public static void DoLoadFromDisk()
        {
            try
            {
                using (FileStream fs = File.Open(WeaponDataManager.CUSTOM_ARCHETYPEDATA_PATH, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        string jsonContent = reader.ReadToEnd();
                        JsonConverter converter = new();
                        Wrapper = converter.Deserialize<CustomGameDataBlockWrapper<CustomArchetypeDataBlock>>(jsonContent);
                        s_blockByID.Clear();
                        s_blockByID = Wrapper.Blocks.ToDictionary(block => block.persistentID, block => block);
                    }
                }
                GameEventLogManager.Current.AddLog("<color=orange>[WeaponDataManager]</color> <color=green>已读取 CustomArchetypeDataBlocks</color>");
            }
            catch (Exception ex)
            {
                Logs.LogError(ex.Message);
                GameEventLogManager.Current.AddLog("<color=orange>[WeaponDataManager]</color> <color=red>读取 CustomArchetypeDataBlocks 失败, 请查看 Log 获取更多信息!</color>");
            }
        }
    }

    [Serializable]
    internal class CustomRecoilDataBlock : CustomGameDataBlockBase<CustomRecoilDataBlock>
    {
        public MinMaxValue power { get; set; }

        public float spring { get; set; }

        public float dampening { get; set; }

        public float hipFireCrosshairSizeDefault { get; set; }

        public float hipFireCrosshairRecoilPop { get; set; }

        public float hipFireCrosshairSizeMax { get; set; }

        public MinMaxValue horizontalScale { get; set; }

        public MinMaxValue verticalScale { get; set; }

        public float directionalSimilarity { get; set; }

        public float worldToViewSpaceBlendHorizontal { get; set; }

        public float worldToViewSpaceBlendVertical { get; set; }

        public Vector3 recoilPosImpulse { get; set; }

        public Vector3 recoilPosShift { get; set; }

        public float recoilPosShiftWeight { get; set; }

        public float recoilPosStiffness { get; set; }

        public float recoilPosDamping { get; set; }

        public float recoilPosImpulseWeight { get; set; }

        public float recoilCameraPosWeight { get; set; }

        public float recoilAimingWeight { get; set; }

        public Vector3 recoilRotImpulse { get; set; }

        public float recoilRotStiffness { get; set; }

        public float recoilRotDamping { get; set; }

        public float recoilRotImpulseWeight { get; set; }

        public float recoilCameraRotWeight { get; set; }

        public float concussionIntensity { get; set; }

        public float concussionFrequency { get; set; }

        public float concussionDuration { get; set; }

        public static void DoLoadFromDisk()
        {
            try
            {
                using (FileStream fs = File.Open(WeaponDataManager.CUSTOM_RECOILDATA_PATH, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        string jsonContent = reader.ReadToEnd();
                        JsonConverter converter = new();
                        Wrapper = converter.Deserialize<CustomGameDataBlockWrapper<CustomRecoilDataBlock>>(jsonContent);
                        s_blockByID.Clear();
                        s_blockByID = Wrapper.Blocks.ToDictionary(block => block.persistentID, block => block);
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
    }

    [Serializable]
    internal class CustomWeaponAudioDataBlock : CustomGameDataBlockBase<CustomWeaponAudioDataBlock>
    {
        public List<string> eventOnSemiFire2D { get; set; }

        public List<string> eventOnBurstFire2D { get; set; }

        public List<string> eventOnAutoFireStart2D { get; set; }

        public List<string> eventOnAutoFireEnd2D { get; set; }

        public List<string> eventOnBurstFireOneShot2D { get; set; }

        public List<string> eventOnChargeup2D { get; set; }

        public List<string> eventOnCooldown2D { get; set; }

        public string eventOnChargeupEnd2D { get; set; }

        public string eventOnCooldownEnd2D { get; set; }

        public List<string> eventOnSemiFire3D { get; set; }

        public List<string> eventOnBurstFire3D { get; set; }

        public List<string> eventOnAutoFireStart3D { get; set; }

        public List<string> eventOnAutoFireEnd3D { get; set; }

        public List<string> eventOnChargeup3D { get; set; }

        public List<string> eventOnCooldown3D { get; set; }

        public string eventOnChargeupEnd3D { get; set; }

        public string eventOnCooldownEnd3D { get; set; }

        public List<string> eventOnSyncedBurstFirePerShot3D { get; set; }

        public List<string> eventOnSyncedAutoFirePerShot3D { get; set; }

        public string eventClick { get; set; }

        public string eventReload { get; set; }

        public bool TriggerBurstAudioForEachShot { get; set; }

        public bool TriggerAutoAudioForEachShot { get; set; }

        public string eventEquip { get; set; }

        public string eventZoomIn { get; set; }

        public string eventZoomOut { get; set; }

        public static void DoLoadFromDisk()
        {
            try
            {
                using (FileStream fs = File.Open(WeaponDataManager.CUSTOM_WEAPONAUDIODATA_PATH, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        string jsonContent = reader.ReadToEnd();
                        JsonConverter converter = new();
                        Wrapper = converter.Deserialize<CustomGameDataBlockWrapper<CustomWeaponAudioDataBlock>>(jsonContent);
                        s_blockByID.Clear();
                        s_blockByID = Wrapper.Blocks.ToDictionary(block => block.persistentID, block => block);
                    }
                }
                GameEventLogManager.Current.AddLog("<color=orange>[WeaponDataManager]</color> <color=green>已读取 CustomWeaponAudioDataBlocks</color>");
            }
            catch (Exception ex)
            {
                Logs.LogError(ex.Message);
                GameEventLogManager.Current.AddLog("<color=orange>[WeaponDataManager]</color> <color=red>读取 CustomWeaponAudioDataBlocks 失败, 请查看 Log 获取更多信息!</color>");
            }
        }
    }
}
