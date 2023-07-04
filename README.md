## 简介

为 ArchtypeDataBlock, RecoilDataBlock 与 WeaponAudioDataBlock 提供实时热重载支持, 可在游戏中实现武器数据的重载, 重载后立即生效

为 BulletWeapon 提供开火模式实时切换支持, 切换后立即生效, 无需切换武器或重置后备数据

本插件主要面向 DataBlock 和 Rundown 创作者

## 注意

本插件会阻止你加入正常玩家的大厅, 也会阻止正常玩家加入你的大厅

## 配置文件说明

SwitchFireModeKey: 切换开火模式热键

ReloadCustomDataKey: 热重载自定义数据热键

EnableSwitchFireModeSound: 是否启用开火模式切换音效

EnableHint: 是否启用开火模式切换提示

## 数据结构说明

主要有四个 DataBlock:
 - CustomGearCategoryDataBlock
 - CustomArchetypeDataBlock
 - CustomRecoilDataBlock
 - CustomWeaponAudioDataBlock
 
对于需要启用的 DataBlock 务必将所有相关的 internalEnabled 设置为 true, 设为 false 则为不启用

### CustomGearCategoryDataBlock 示例:
```
{
    "Headers": [],
    "Blocks": [
    {
      "ArchetypeDataSequence": [
        22,
        21,
        23
      ],
      "FireModeNameSequence": [
        "连发",
        "单发",
        "全自动"
      ],
      "FireModeAudioSequence": [
        43,
        43,
        43
      ],
      "SwitchFireModeAudioEventID": 0,
      "OriginAmmoMax": 331.5,
      "name": "连发炮",
      "internalEnabled": true,
      "persistentID": 3
    }
  ],
  "LastPersistentID": 3
}
```
ArchetypeDataSequence: 开火模式切换序列

FireModeNameSequence: FireModeSequence 对应开火模式的译名, 译名用于在游戏中提供提示

OriginAmmoMax: 原版枪械可携带弹药量的最大值, AmmoMaxCap + ClipSize * CostOfBullet 或者 BulletsMaxCap * CostOfBullet 都可以, 区别不大

persistentID 对应 GameData_GearCategoryDataBlock 的 persistentID, 两者必须保持一致。 除开这一项的其他所有属性, 包括另外三个 CustomDataBlock 中的所有属性 (包括 persistentID) 都可以随意修改

## 更新日志

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
