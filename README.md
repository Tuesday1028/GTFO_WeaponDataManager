## 简介

为 ArchtypeDataBlock, RecoilDataBlock 与 WeaponAudioDataBlock 提供实时热重载支持, 可在游戏中实现武器数据的重载, 重载后立即生效

为 BulletWeapon 提供模式实时切换支持, 切换后立即生效, 无需切换武器或重置后备数据

## 注意

~~本插件会阻止你加入正常玩家的大厅, 也会阻止正常玩家加入你的大厅~~

现在取消这个限制, 希望大家绿色游戏, 不要过分魔改数据影响游戏平衡

## 配置文件说明

SwitchFireModeKey: 切换开火模式热键

ReloadCustomDataKey: 热重载自定义数据热键

EnableSwitchFireModeSound: 是否启用开火模式切换音效

EnableHint: 是否启用开火模式切换提示

## 数据结构说明

四个 CustomDataBlock:
 - CustomGearCategoryDataBlock
 - CustomArchetypeDataBlock
 - CustomRecoilDataBlock
 - CustomWeaponAudioDataBlock
 
对于需要启用的 DataBlock 务必将所有相关的 internalEnabled 设置为 true, 设为 false 则为不启用

### CustomGearCategoryDataBlock 示例:
```
{
 "Headers": [],
 "Blocks": [{
   "WeaponModes": [{
     "ArchetypeID": 22,
     "AudioID": 43,
     "Name": "连发"
    },
    {
     "ArchetypeID": 21,
     "AudioID": 43,
     "Name": "单发"
    },
    {
     "ArchetypeID": 23,
     "AudioID": 43,
     "Name": "全自动"
    }
   ],
   "SwitchFireModeAudioEventID": 0,
   "OriginAmmoMax": 331.5,
   "name": "连发炮",
   "internalEnabled": true,
   "persistentID": 3
  },
  {
   "WeaponModes": [{
     "ArchetypeID": 37,
     "AudioID": 29,
     "Name": "原版"
    },
    {
     "ArchetypeID": 300,
     "AudioID": 29,
     "Name": "专业版"
    }
   ],
   "SwitchFireModeAudioEventID": 0,
   "OriginAmmoMax": 254.8,
   "name": "专业左轮",
   "internalEnabled": true,
   "persistentID": 7
  },
  {
   "WeaponModes": [{
     "ArchetypeID": 41,
     "AudioID": 1,
     "Name": "原版"
    },
    {
     "ArchetypeID": 42,
     "AudioID": 1,
     "Name": "连发"
    },
    {
     "ArchetypeID": 43,
     "AudioID": 1,
     "Name": "全自动"
    }
   ],
   "SwitchFireModeAudioEventID": 0,
   "OriginAmmoMax": 504,
   "name": "手枪",
   "internalEnabled": true,
   "persistentID": 8
  }
 ],
 "LastPersistentID": 8
}
```
WeaponModes: 武器模式, 顺序表示模式切换顺序, 第一个为默认模式
 - ArchetypeID: CustomArchetypeDataBlock 的 persistentID
 - AudioID: CustomWeaponAudioDataBlock 的 persistentID
 - Name: 模式名称, 用于游戏内武器模式切换提示

SwitchFireModeAudioEventID: 切换武器模式的音效ID

OriginAmmoMax: 原版枪械可携带弹药量的最大值, 计算公式: `ClipSize * CostOfBullet + AmmoMaxCap` 或者 `BulletMaxCap * CostOfBullet`, 这个属性是为了修正别人视角中你的弹药量, 务必准确填写

persistentID 对应 GameData_GearCategoryDataBlock 的 persistentID, 两者必须保持一致。 除开这一项的其他所有属性, 包括另外三个 CustomDataBlock 中的所有属性 (包括 persistentID) 都可以随意修改

## 更新日志
v1.1.4
 - 优化 CustomGearCategoryDataBlock 结构

v1.1.3
 - BUG 修复

v1.1.2
 - BUG 修复

v1.1.1
 - BUG 修复

v1.1.0
 - 兼容 MTFO
 - BUG 修复
 - 逻辑改进

v1.0.2
 - BUG 修复
 - 开火方式切换音效可针对每一把武器设置
 - 添加 WeaponAudioDataBlock 替换支持
 - 提示信息可自定义化
 - 重载数据后立即应用到当前枪械

v1.0.1
 - BUG 修复
 - 添加遗漏的音效功能
