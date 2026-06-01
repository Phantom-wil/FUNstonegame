# PasserCard GDD（Phase 1）

## 核心循环

Roguelike 地图（Phase 3）+ 守路人 encounter。Encounter 内：弃牌 → 预钉雾槽 → Pass → 填槽出牌 → 计分，共 4 配额；**单次最高分 ≥ 目标分** 胜利。

## 资源

- **魂币**：无 HP；献祭卡戎、商店、♦ Pass 消耗、战斗奖惩。
- **Pass 次数**：5/encounter（♣ 不占次数）。

## 四花色 Pass

| 花色 | Pass 效果 |
|------|-----------|
| ♠ | +1 点，不计顺子 |
| ♥ | +1 魂币，Heart 层数使 Chips −10%/层（最多 3） |
| ♣ | 不占 Pass 次数；触发迷途（下次 Pass 需双翻） |
| ♦ | −1 魂币，+15 Chips |

## Act 1 迷雾牌桌

- 出牌区 5 槽，每配额刷新 2 雾槽。
- 预钉入雾：锁面，不可 Pass。
- 已 Pass 的牌不可放入雾槽。
- 雾槽内牌不触发花色 Pass 效果。

## 附魔（Phase 2+）

- 战争·裂痕、饥荒·折角、瘟疫·瘟染、死亡·枯寂（焚灭 Mult×2）。

## 代码入口

- `EncounterSession` — 纯 C# encounter 状态机
- `GameBootstrap` — 场景入口，自动启动 UI encounter
- `EncounterUIController` — 手牌、5 槽、阶段按钮
- `Assets/_Project/Tests/` — EditMode 单元测试
