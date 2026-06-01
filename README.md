# PasserCard

Unity 卡牌 Roguelike（C# / 2D / UGUI）。亡魂 Passer 持魂币赴冥王星，以 **双面扑克 + Pass + 迷雾出牌槽** 对抗守路人。

## 环境

- Unity **2021.2.17f1**
- 可选：Rider / Visual Studio

## 打开项目

1. Unity Hub → **Add** → 本仓库根目录
2. 打开 `Assets/_Project/Scenes/Main.unity`（已挂载 `GameBootstrap`）
3. 点 **▶ Play** — 自动显示 encounter UI

## 已实现

| 模块 | 说明 |
|------|------|
| `PlayingCardInstance` | 双角点数、Pass 面、配额/迷雾 flags |
| `PokerHandEvaluator` | 1～5 张德扑牌型计分 |
| `SuitPassRules` | 四花色 Pass 规则 |
| `PlayArea` | 5 出牌槽 + 每配额迷雾刷新 |
| `EncounterSession` | 4 出牌 / 3 弃牌 / 5 Pass |
| `EncounterUIController` | 运行时 UGUI，完整可玩流程 |
| `BalatroSpriteLibrary` | 临时 Balatro 像素牌面（Resources 切图） |
| `PasserCard.Tests` | EditMode 单元测试 |

## 临时美术（Balatro）

原型阶段使用 Balatro 风格像素素材（**仅本地开发，发布前请替换**）：

- `Assets/_Project/Resources/Balatro/8BitDeck.png` — 标准 52 张牌面（14×4 网格，末列为花色大图标）
- `Assets/_Project/Resources/Balatro/CardFace.png` — 空白羊皮纸卡面（牌面底图）
- `Assets/_Project/Resources/Balatro/Enhancers.png` — 卡背（迷雾空槽）
- `Assets/_Project/Resources/Balatro/UIAssets.png` — 筹码等小图标

首次在 Unity 中打开项目时会自动设置 **Point 过滤 + Read/Write**（见 `Editor/BalatroTextureImportPostprocessor.cs`）。若牌面不显示，在 Project 窗口选中上述 PNG → **Reimport**。

## 操作说明

1. **点击手牌**选中（再点取消）
2. **弃牌** → 换一张（整局 3 次）
3. **预钉雾槽** → 选牌点 **雾槽**（锁面，不可 Pass）
4. **→ Pass** → **Pass 选中** 翻转手牌
5. **→ 出牌** → 选牌点 **空槽**（已 Pass 的牌不能进雾槽）
6. **计分** → **下一配额**（共 4 次出牌机会）
7. **最高分 ≥ 120**（默认）即胜利

## 目录

```
Assets/_Project/Scripts/
  Cards/       扑克与工厂
  Poker/       牌型判定
  Pass/        花色 Pass
  Table/       出牌槽与迷雾
  Encounter/   战斗会话
  Run/         魂币
  Core/        GameBootstrap
  UI/          EncounterUIController, EncounterUIBuilder, BalatroSpriteLibrary
Assets/_Project/Resources/Balatro/  临时牌面图集
Assets/_Project/Editor/             纹理导入设置
Assets/_Project/Tests/
docs/GDD.md
```

## 测试

Unity Test Runner → EditMode → 运行 `PasserCard.Tests`。

## 文档

- [GDD](docs/GDD.md)
- [ROADMAP](docs/ROADMAP.md)
