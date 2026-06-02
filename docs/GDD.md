# PasserCard GDD（Phase 1–3）

## 核心循环

Roguelike 地图 + 守路人 encounter。Encounter 内：弃牌 → 预钉雾槽 → Pass → 填槽出牌 → 计分，共 4 配额；**单次最高分 ≥ 目标分** 胜利。

## 资源

- **魂币**：无 HP；献祭卡戎、商店、♦ Pass 消耗、战斗奖惩。
- **Pass 次数**：5/encounter（♣ 不占次数；职业可 +1）。

## 四花色 Pass

| 花色 | Pass 效果 |
|------|-----------|
| ♠ | +1 点，不计顺子 |
| ♥ | +1 魂币，Heart 层数使 Chips −10%/层（最多 3） |
| ♣ | 不占 Pass 次数；触发迷途（下次 Pass 需双翻） |
| ♦ | −1 魂币，+15 Chips |

## 牌桌环境（按 Act）

| 牌桌 | 规则 |
|------|------|
| 迷雾（Act 1） | 5 槽 · 每配额 2 雾槽 · 入雾不可 Pass |
| 光滑（Act 2+） | Pass 后 25% 额外翻转 |
| 荆棘（Act 2+） | Pass 耗 1 魂币；未 Pass 出牌扣 1 魂币 |
| 裂隙（Act 2+） | 战争裂痕牌 Chips 加成提升 |

## 附魔（四骑士）

| 骑士 | 效果 |
|------|------|
| 战争·裂痕 | 35% 分裂为半张，牌库 +1 |
| 饥荒·折角 | 锁定面，不可 Pass |
| 瘟疫·瘟染 | Pass 后污染左右相邻牌 |
| 死亡·枯寂 | 打出后焚灭；最后配额 Mult×2 |

## Run 构筑

- **身份**：摆渡者 / 裂魂者 / 守财灵
- **套牌**：标准 / 冥河 / 商旅

## 地图

STS 式分支：守路人、精英、商店、星球、卡戎、Boss。

## Boss

| Boss | 机制 |
|------|------|
| 尼克斯 | 3 雾槽 · 雾槽隐藏非生效角 |
| 许德拉 | 6 出牌配额 · 反击更重 |
| 刻耳柏洛斯 | Pass 上限 3 · 未 Pass 额外扣魂币 |
| 斯提克斯 | 光滑牌桌 · Pass 上限 4 |

## 代码入口

| 组件 | 用途 |
|------|------|
| `RunBootstrap` | 完整 Run：地图 + encounter |
| `GameBootstrap` | 单 encounter 测试 |
| `EncounterSession` | encounter 状态机 |
| `MapGenerator` | Act 地图生成 |
| `BossLibrary` | Boss encounter 配置 |
| `RunIdentityLibrary` / `StartingDeckLibrary` | Run 入口 |
| `Assets/_Project/Tests/` | EditMode 单元测试 |
