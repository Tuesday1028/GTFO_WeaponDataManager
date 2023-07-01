## 简介

为 ArchtypeDataBlock 与 RecoilDataBlock 提供实时热重载支持，即可以在游戏中实现武器数据的重载，重载后立即生效

为 BulletWeapon 提供开火模式实时切换支持，即切换后立即生效, 切换的原理是将 ArchtypeDataBlock 与 RecoilDataBlock 替换

本插件不对游戏自身 DataBlock 做任何修改

本插件主要面向 DataBlock 和 Rundown 创作者

## 注意

本插件会阻止你加入正常玩家的大厅，也会阻止正常玩家加入你的大厅

## 配置文件说明

SwitchFireModeKey: 切换开火模式的热键

ReloadCustomDataKey: 热重载自定义数据的热键

EnableSwitchFireModeSound: 是否启用切换开火模式的音效 //暂不可用

SwitchFireModeSoundEventID: 音效ID //暂不可用

## 数据结构说明

主要有三个 DataBlock:
 - CustomGearCategoryDataBlock
 - CustomArchetypeDataBlock
 - CustomRecoilDataBlock
 
对于需要启用的 DataBlock 务必将所有相关的 internalEnabled 设置为 true, 设为 false 则为不启用

### CustomGearCategoryDataBlock 示例:
```
[
  {
    "ArchetypeDatas": [
      22,
	    21,
      23,
	    23
    ],
    "FireModeSequence": [
      1,
      0,
      2,
	    2
    ],
    "FireModeNameSequence": [
      "连发",
	    "单发",
      "全自动_1"
	    "全自动_2"
    ],
	"OriginAmmoMax": 331.5,
    "name": "连发炮",
    "internalEnabled": true,
    "persistentID": 3
  }
]
```
FireModeSequence 存储开火模式切换序列, 第一位为默认值, 其中的值为 eWeaponFireMode 的对应 Index, 允许出现重复的值

FireModeNameSequence 存储 FireModeSequence 对应开火模式的译名, 译名用于在游戏中提供提示

OriginAmmoMax 存储原版枪械可携带弹药量的最大值, 计算公式: BulletsMaxCap * CostOfBullet

ArchetypeDatas 存储 FireModeSequence 对应顺序所使用的 CustomArchetypeDataBlock 的 persistentID

persistentID 对应游戏原版数据 GearCategoryDataBlock 的 persistentID, 不可修改, 两者必须保持一致。 除开这一项的其他所有属性, 包括另外两个 CustomDataBlock 中的所有属性 (包括 persistentID) 都可以随意修改

### CustomArchetypeDataBlock 示例:
```
[
  {
    "PublicName": "连发炮",
    "Description": "单发连发炮",
    "FireMode": 0,
    "RecoilDataID": 24,
    "DamageBoosterEffect": 61,
    "Damage": 20.01,
    "DamageFalloff": {
      "x": 15.0,
      "y": 80.0,
      "normalized": {
        "x": 0.184288532,
        "y": 0.9828722,
        "magnitude": 1.0,
        "sqrMagnitude": 1.0
      },
      "magnitude": 81.394104,
      "sqrMagnitude": 6625.0
    },
    "StaggerDamageMulti": 1.0,
    "PrecisionDamageMulti": 0.734,
    "DefaultClipSize": 20,
    "DefaultReloadTime": 3.7,
    "CostOfBullet": 5.1,
    "ShotDelay": 0.06,
    "ShellCasingSize": 1.0,
    "ShellCasingSpeedRange": {
      "x": 3.0,
      "y": 5.0,
      "normalized": {
        "x": 0.5144958,
        "y": 0.857493,
        "normalized": {
          "x": 0.51449573,
          "y": 0.857492864,
          "normalized": {
            "x": 0.5144958,
            "y": 0.8574929,
            "magnitude": 1.0,
            "sqrMagnitude": 1.0
          },
          "magnitude": 0.99999994,
          "sqrMagnitude": 0.9999999
        },
        "magnitude": 1.00000012,
        "sqrMagnitude": 1.00000012
      },
      "magnitude": 5.83095169,
      "sqrMagnitude": 34.0
    },
    "PiercingBullets": false,
    "PiercingDamageCountLimit": 5,
    "HipFireSpread": 2.0,
    "AimSpread": 0.1,
    "EquipTransitionTime": 0.6,
    "EquipSequence": [
      {
        "TriggerTime": 0.0,
        "Type": 0,
        "StringData": "Front_Machinegun_1_equip_weaponmovement"
      },
      {
        "TriggerTime": 2.0,
        "Type": 7
      }
    ],
    "AimTransitionTime": 0.35,
    "AimSequence": [],
    "BurstDelay": 0,
    "BurstShotCount": 5,
    "ShotgunBulletCount": 0,
    "ShotgunConeSize": 0,
    "ShotgunBulletSpread": 0,
    "SpecialChargetupTime": 0.0,
    "SpecialCooldownTime": 0.0,
    "SpecialSemiBurstCountTimeout": 0.5,
    "Sentry_StartFireDelay": 1.0,
    "Sentry_RotationSpeed": 4.0,
    "Sentry_DetectionMaxRange": 25.0,
    "Sentry_DetectionMaxAngle": 30.0,
    "Sentry_FireTowardsTargetInsteadOfForward": false,
    "Sentry_LongRangeThreshold": 999.0,
    "Sentry_ShortRangeThreshold": 0.0,
    "Sentry_LegacyEnemyDetection": true,
    "Sentry_FireTagOnly": false,
    "Sentry_PrioTag": false,
    "Sentry_StartFireDelayTagMulti": 1.0,
    "Sentry_RotationSpeedTagMulti": 1.0,
    "Sentry_DamageTagMulti": 1.0,
    "Sentry_StaggerDamageTagMulti": 1.0,
    "Sentry_CostOfBulletTagMulti": 1.0,
    "Sentry_ShotDelayTagMulti": 1.0,
    "name": "连发炮-单发",
    "internalEnabled": true,
    "persistentID": 21
  },
  {
    "PublicName": "连发炮",
    "Description": "连发连发炮",
    "FireMode": 1,
    "RecoilDataID": 24,
    "DamageBoosterEffect": 61,
    "Damage": 19.0,
    "DamageFalloff": {
      "x": 15.0,
      "y": 80.0,
      "normalized": {
        "x": 0.184288532,
        "y": 0.9828722,
        "magnitude": 1.0,
        "sqrMagnitude": 1.0
      },
      "magnitude": 81.394104,
      "sqrMagnitude": 6625.0
    },
    "StaggerDamageMulti": 1.0,
    "PrecisionDamageMulti": 0.734,
    "DefaultClipSize": 20,
    "DefaultReloadTime": 3.7,
    "CostOfBullet": 5.1,
    "ShotDelay": 0.06,
    "ShellCasingSize": 1.0,
    "ShellCasingSpeedRange": {
      "x": 3.0,
      "y": 5.0,
      "normalized": {
        "x": 0.5144958,
        "y": 0.857493,
        "normalized": {
          "x": 0.51449573,
          "y": 0.857492864,
          "normalized": {
            "x": 0.5144958,
            "y": 0.8574929,
            "magnitude": 1.0,
            "sqrMagnitude": 1.0
          },
          "magnitude": 0.99999994,
          "sqrMagnitude": 0.9999999
        },
        "magnitude": 1.00000012,
        "sqrMagnitude": 1.00000012
      },
      "magnitude": 5.83095169,
      "sqrMagnitude": 34.0
    },
    "PiercingBullets": false,
    "PiercingDamageCountLimit": 5,
    "HipFireSpread": 2.0,
    "AimSpread": 0.1,
    "EquipTransitionTime": 0.6,
    "EquipSequence": [
      {
        "TriggerTime": 0.0,
        "Type": 0,
        "StringData": "Front_Machinegun_1_equip_weaponmovement"
      },
      {
        "TriggerTime": 2.0,
        "Type": 7
      }
    ],
    "AimTransitionTime": 0.35,
    "AimSequence": [],
    "BurstDelay": 0.3,
    "BurstShotCount": 5,
    "ShotgunBulletCount": 0,
    "ShotgunConeSize": 0,
    "ShotgunBulletSpread": 0,
    "SpecialChargetupTime": 0.7,
    "SpecialCooldownTime": 0.0,
    "SpecialSemiBurstCountTimeout": 0.5,
    "Sentry_StartFireDelay": 1.0,
    "Sentry_RotationSpeed": 4.0,
    "Sentry_DetectionMaxRange": 25.0,
    "Sentry_DetectionMaxAngle": 30.0,
    "Sentry_FireTowardsTargetInsteadOfForward": false,
    "Sentry_LongRangeThreshold": 999.0,
    "Sentry_ShortRangeThreshold": 0.0,
    "Sentry_LegacyEnemyDetection": true,
    "Sentry_FireTagOnly": false,
    "Sentry_PrioTag": false,
    "Sentry_StartFireDelayTagMulti": 1.0,
    "Sentry_RotationSpeedTagMulti": 1.0,
    "Sentry_DamageTagMulti": 1.0,
    "Sentry_StaggerDamageTagMulti": 1.0,
    "Sentry_CostOfBulletTagMulti": 1.0,
    "Sentry_ShotDelayTagMulti": 1.0,
    "name": "连发炮-连发",
    "internalEnabled": true,
    "persistentID": 22
  },
  {
    "PublicName": "连发炮",
    "Description": "全自动连发炮",
    "FireMode": 2,
    "RecoilDataID": 24,
    "DamageBoosterEffect": 61,
    "Damage": 19.0,
    "DamageFalloff": {
      "x": 15.0,
      "y": 80.0,
      "normalized": {
        "x": 0.184288532,
        "y": 0.9828722,
        "magnitude": 1.0,
        "sqrMagnitude": 1.0
      },
      "magnitude": 81.394104,
      "sqrMagnitude": 6625.0
    },
    "StaggerDamageMulti": 1.0,
    "PrecisionDamageMulti": 0.734,
    "DefaultClipSize": 20,
    "DefaultReloadTime": 3.7,
    "CostOfBullet": 5.1,
    "ShotDelay": 0.06,
    "ShellCasingSize": 1.0,
    "ShellCasingSpeedRange": {
      "x": 3.0,
      "y": 5.0,
      "normalized": {
        "x": 0.5144958,
        "y": 0.857493,
        "normalized": {
          "x": 0.51449573,
          "y": 0.857492864,
          "normalized": {
            "x": 0.5144958,
            "y": 0.8574929,
            "magnitude": 1.0,
            "sqrMagnitude": 1.0
          },
          "magnitude": 0.99999994,
          "sqrMagnitude": 0.9999999
        },
        "magnitude": 1.00000012,
        "sqrMagnitude": 1.00000012
      },
      "magnitude": 5.83095169,
      "sqrMagnitude": 34.0
    },
    "PiercingBullets": false,
    "PiercingDamageCountLimit": 5,
    "HipFireSpread": 2.0,
    "AimSpread": 0.1,
    "EquipTransitionTime": 0.6,
    "EquipSequence": [
      {
        "TriggerTime": 0.0,
        "Type": 0,
        "StringData": "Front_Machinegun_1_equip_weaponmovement"
      },
      {
        "TriggerTime": 2.0,
        "Type": 7
      }
    ],
    "AimTransitionTime": 0.35,
    "AimSequence": [],
    "BurstDelay": 0,
    "BurstShotCount": 5,
    "ShotgunBulletCount": 0,
    "ShotgunConeSize": 0,
    "ShotgunBulletSpread": 0,
    "SpecialChargetupTime": 0.0,
    "SpecialCooldownTime": 0.0,
    "SpecialSemiBurstCountTimeout": 0.5,
    "Sentry_StartFireDelay": 1.0,
    "Sentry_RotationSpeed": 4.0,
    "Sentry_DetectionMaxRange": 25.0,
    "Sentry_DetectionMaxAngle": 30.0,
    "Sentry_FireTowardsTargetInsteadOfForward": false,
    "Sentry_LongRangeThreshold": 999.0,
    "Sentry_ShortRangeThreshold": 0.0,
    "Sentry_LegacyEnemyDetection": true,
    "Sentry_FireTagOnly": false,
    "Sentry_PrioTag": false,
    "Sentry_StartFireDelayTagMulti": 1.0,
    "Sentry_RotationSpeedTagMulti": 1.0,
    "Sentry_DamageTagMulti": 1.0,
    "Sentry_StaggerDamageTagMulti": 1.0,
    "Sentry_CostOfBulletTagMulti": 1.0,
    "Sentry_ShotDelayTagMulti": 1.0,
    "name": "连发炮-全自动",
    "internalEnabled": true,
    "persistentID": 23
  }
]
```

数据结构为使用 MTFO Dump 出的数据去掉头尾保留中间部分

RecoilDataID 表示需要使用的 CustomRecoilDataBlock 的 persistentID

### CustomRecoilDataBlock 示例:
```
[
  {
    "power": {
      "Min": 0.6,
      "Max": 1.2
    },
    "spring": 2.0,
    "dampening": 20.0,
    "hipFireCrosshairSizeDefault": 70.0,
    "hipFireCrosshairRecoilPop": 10.0,
    "hipFireCrosshairSizeMax": 90.0,
    "horizontalScale": {
      "Min": -0.35,
      "Max": 0.1
    },
    "verticalScale": {
      "Min": 0.8,
      "Max": 1.0
    },
    "directionalSimilarity": 0.0,
    "worldToViewSpaceBlendHorizontal": 0.3,
    "worldToViewSpaceBlendVertical": 0.3,
    "recoilPosImpulse": {
      "x": 0.0,
      "y": 0.0,
      "z": -0.42,
      "normalized": {
        "x": 0.0,
        "y": 0.0,
        "z": -1.0,
        "magnitude": 1.0,
        "sqrMagnitude": 1.0
      },
      "magnitude": 0.42,
      "sqrMagnitude": 0.176399991
    },
    "recoilPosShift": {
      "x": 0.0,
      "y": -0.1,
      "z": -0.03,
      "normalized": {
        "x": 0.0,
        "y": -0.9578263,
        "z": -0.287347883,
        "magnitude": 1.0,
        "sqrMagnitude": 1.0
      },
      "magnitude": 0.104403064,
      "sqrMagnitude": 0.0109
    },
    "recoilPosShiftWeight": 1.0,
    "recoilPosStiffness": 350.0,
    "recoilPosDamping": 25.0,
    "recoilPosImpulseWeight": 1.0,
    "recoilCameraPosWeight": 1.0,
    "recoilAimingWeight": 2.1,
    "recoilRotImpulse": {
      "x": -1.0,
      "y": 1.0,
      "z": 1.0,
      "normalized": {
        "x": -0.577350259,
        "y": 0.577350259,
        "z": 0.577350259,
        "magnitude": 1.0,
        "sqrMagnitude": 0.99999994
      },
      "magnitude": 1.73205078,
      "sqrMagnitude": 3.0
    },
    "recoilRotStiffness": 90.0,
    "recoilRotDamping": 8.0,
    "recoilRotImpulseWeight": 230.0,
    "recoilCameraRotWeight": 1.0,
    "concussionIntensity": 0.0,
    "concussionFrequency": 0.0,
    "concussionDuration": 0.0,
    "name": "连发炮-连发",
    "internalEnabled": true,
    "persistentID": 24
  }
]
```

数据结构为使用 MTFO Dump 出的数据去掉头尾保留中间部分

