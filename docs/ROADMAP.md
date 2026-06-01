# PasserCard — 前期规划

## 游戏类型

Unity 2D 卡牌游戏。

## 当前阶段（v0.1）

- [x] 项目重命名为 PasserCard
- [x] Unity 目录与 `.gitignore`
- [x] 双面扑克 + Pass + 四花色规则
- [x] 德扑牌型 evaluator + 单元测试
- [x] 迷雾出牌槽 + EncounterSession（4/3/5 配额）
- [x] `GameBootstrap` Demo 入口
- [x] 卡牌 UI（运行时 UGUI：手牌、5 出牌槽、Pass/弃牌/计分按钮）
- [ ] STS 式 Act 地图
- [ ] 四骑士附魔完整实现
- [ ] 音效与基础动画

## 技术选型

| 领域 | 选择 |
|------|------|
| 渲染 | 2D Sprite + UGUI |
| 文本 | TextMeshPro |
| 输入 | Unity Input System |
| 数据 | ScriptableObject + 运行时 C# 模型 |

## 后续模块（待设计）

- **Battle** — 战场、目标选择、伤害结算
- **AI** — 对手决策
- **Meta** — 卡组构建、收藏、进度存档
- **Network** — 若需要联机，单独评估

## 文件夹说明

空目录用 `.gitkeep` 占位，便于 Unity 与 Git 跟踪：

- `Art/` — 卡牌框、背景、图标
- `Audio/` — BGM、出牌/攻击音效
- `Data/Cards/` — `CardDefinition` 资产
- `Prefabs/` — `CardView` 等 UI 预制体
- `Scenes/` — `Main.unity` 主场景
