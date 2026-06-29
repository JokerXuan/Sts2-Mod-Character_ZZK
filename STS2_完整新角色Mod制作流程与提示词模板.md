# 《杀戮尖塔2》完整新角色 Mod 制作流程与提示词模板

> 目标：制作一个完整可玩的《杀戮尖塔2》（Slay the Spire 2）新角色 Mod，包含角色设定、原画/卡图、战斗动画、新卡牌、新遗物、新药水、能力/关键词、必要的事件/怪物/内容扩展、调试测试与发布流程。  
> 推荐技术路线：Godot 4.5.1 .NET + C# + RitsuLib + Harmony Patch（必要时）。  
> 适用范围：先完成“最小可玩角色 MVP”，再扩展为完整内容包。

---

## 0. 总体思路：先做可运行，再做完整

完整角色 Mod 不应该一开始就同时做 80 张卡、全套动画、事件和怪物。正确顺序是：

1. **空 Mod 能加载**：游戏能识别你的 Mod。
2. **新角色能进入角色选择界面**：有角色名、描述、初始血量、能量、卡池、初始卡组、初始遗物。
3. **最小卡组能打完一场战斗**：至少有攻击、防御、1 张机制牌、1 个关键词或能力。
4. **机制闭环成立**：角色核心资源/状态/牌组逻辑可以稳定运行。
5. **扩充卡池**：从 10 张 → 30 张 → 60~80 张。
6. **补齐资产**：角色立绘、战斗图、动画、卡图、遗物图、药水图、UI 图标、能量图标。
7. **补齐内容**：遗物、药水、事件、怪物、可选剧情/时间线。
8. **测试、平衡、发布**。

建议把项目拆成四个里程碑：

| 里程碑 | 目标 | 完成标准 |
|---|---|---|
| M0 空 Mod | 环境配置与 Mod 加载 | 游戏 Mod 列表能看到你的 Mod |
| M1 可选角色 | 新角色能进入游戏 | 角色选择界面可见，能开局 |
| M2 机制 MVP | 10~15 张牌 + 1 个核心机制 | 可完整打一局，不报错 |
| M3 完整角色 | 60~80 张牌 + 全套资产 + 遗物/药水/事件 | 接近创意工坊发布标准 |

---

## 1. 技术栈选择

### 1.1 必备工具

需要准备：

- 《杀戮尖塔2》Steam 版；
- Godot 4.5.1 .NET / Mono 版本；
- .NET SDK 9.0 或更高；
- C# IDE：Rider、Visual Studio、VS Code 均可；
- RitsuLib 或 BaseLib；
- 图片编辑工具：Krita、Photoshop、CSP、Procreate 等；
- 可选：Spine、GDRE/gdsdecomp、ILSpy/dnSpy；
- 可选：版本管理 Git。

### 1.2 为什么推荐 RitsuLib

完整新角色会涉及：

- 新角色注册；
- 自定义卡池、遗物池、药水池；
- 新卡牌、新能力、新药水、新遗物；
- 角色动画状态机；
- 自定义配置；
- 局内数据保存；
- 内容注册；
- 热键、组件、数据持久化等扩展。

RitsuLib 对这些角色 Mod 常用能力覆盖更完整，尤其是“人物非 Spine 动画兼容”“动画状态机”“生命周期事件”“数据持久化”“自定义目标”等能力，对完整角色项目更合适。

---

## 2. 项目结构建议

建议项目命名不要使用中文、空格或特殊符号。假设：

- ModID：`ChronoSeer`
- 角色名：`时视者`
- 命名空间：`ChronoSeer.Scripts`
- 根资源目录：`res://ChronoSeer/`

推荐目录：

```text
ChronoSeer/
├─ ChronoSeer.csproj
├─ ChronoSeer.json
├─ project.godot
├─ Scripts/
│  ├─ Entry.cs
│  ├─ Characters/
│  │  ├─ ChronoSeerCharacter.cs
│  │  ├─ ChronoSeerCardPool.cs
│  │  ├─ ChronoSeerRelicPool.cs
│  │  └─ ChronoSeerPotionPool.cs
│  ├─ Cards/
│  │  ├─ ChronoCardModel.cs
│  │  ├─ Starter/
│  │  ├─ Common/
│  │  ├─ Uncommon/
│  │  ├─ Rare/
│  │  └─ Special/
│  ├─ Powers/
│  ├─ Relics/
│  ├─ Potions/
│  ├─ Keywords/
│  ├─ Events/
│  ├─ Monsters/
│  ├─ Patches/
│  └─ Utils/
├─ ChronoSeer/
│  ├─ images/
│  │  ├─ cards/
│  │  ├─ character/
│  │  ├─ relics/
│  │  ├─ potions/
│  │  ├─ ui/
│  │  └─ vfx/
│  ├─ scenes/
│  │  ├─ characters/
│  │  ├─ ui/
│  │  └─ vfx/
│  ├─ audio/
│  └─ localization/
│     ├─ zhs/
│     │  ├─ cards.json
│     │  ├─ characters.json
│     │  ├─ relics.json
│     │  ├─ powers.json
│     │  ├─ potions.json
│     │  └─ keywords.json
│     └─ eng/
└─ docs/
   ├─ design.md
   ├─ card_table.md
   ├─ balance_log.md
   └─ test_plan.md
```

---

## 3. 阶段一：创建空 Mod 并让游戏加载

### 3.1 创建 Godot 项目

1. 打开 Godot 4.5.1 .NET。
2. 新建项目，项目名建议与 ModID 一致，例如 `ChronoSeer`。
3. 渲染器建议选择 Mobile/移动，以尽量贴近游戏环境。
4. 创建 C# 解决方案。

### 3.2 创建 `ChronoSeer.json`

示例：

```json
{
  "id": "ChronoSeer",
  "name": "Chrono Seer",
  "author": "你的名字",
  "description": "Adds a new playable character: Chrono Seer.",
  "version": "0.1.0",
  "min_game_version": "0.107.1",
  "has_pck": true,
  "has_dll": true,
  "dependencies": [
    { "id": "STS2-RitsuLib", "min_version": "0.2.27" }
  ],
  "affects_gameplay": true
}
```

注意：

- `id` 必须唯一；
- 版本号必须是 `X.X.X` 三段式；
- 依赖 RitsuLib 时必须写入 `dependencies`；
- 如果你的版本不同，`min_game_version` 与 RitsuLib 版本要按实际情况调整。

### 3.3 修改 `.csproj`

关键点：

- TargetFramework 使用 `net9.0`；
- 引用游戏目录中的 `sts2.dll` 与 `0Harmony.dll`；
- 引入 `STS2.RitsuLib`；
- 构建后自动复制 dll 和 json 到游戏 `mods` 文件夹；
- 后续可以添加 Godot headless 导出 PCK 的命令。

### 3.4 创建入口类 `Entry.cs`

入口类负责：

- 注册 Mod；
- 初始化 RitsuLib；
- 自动发现本 Mod 的内容类；
- 必要时启用 Harmony Patch；
- 输出日志。

核心逻辑是：

```csharp
var assembly = Assembly.GetExecutingAssembly();
RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
```

### 3.5 构建和验证

执行：

```bash
dotnet build
```

然后确认游戏目录：

```text
Slay the Spire 2/mods/ChronoSeer/
├─ ChronoSeer.dll
├─ ChronoSeer.json
└─ ChronoSeer.pck
```

运行游戏。第一次开启 Mod 后游戏可能会重启；第二次进入后，右下角或 Mod 列表应能看到你的 Mod。

---

## 4. 阶段二：创建新角色骨架

### 4.1 创建三个角色专属池

完整角色至少需要：

- `ChronoSeerCardPool.cs`
- `ChronoSeerRelicPool.cs`
- `ChronoSeerPotionPool.cs`

它们用于区分该角色自己的：

- 卡牌池；
- 遗物池；
- 药水池；
- 能量图标；
- 卡框颜色；
- 卡池主题色。

卡牌池要定义：

- `Title`
- `EnergyColorName`
- 小能量图标路径；
- 大能量图标路径；
- 牌组入口颜色；
- 能量轮廓颜色；
- 是否无色；
- 卡框材质或色调替换策略。

### 4.2 创建角色类

例如：

```text
Scripts/Characters/ChronoSeerCharacter.cs
```

角色类应继承类似：

```csharp
ModCharacterTemplate<ChronoSeerCardPool, ChronoSeerRelicPool, ChronoSeerPotionPool>
```

角色类要处理：

- `[RegisterCharacter]` 注册；
- 角色名称颜色；
- 能量图标轮廓颜色；
- 地图绘制颜色；
- 初始血量；
- 初始金币；
- 初始能量；
- 初始卡组；
- 初始遗物；
- 角色选择界面资源；
- 战斗视觉资源；
- 攻击/施法动画延迟；
- 是否需要时间线小故事；
- 战斗特效列表。

### 4.3 创建角色本地化

创建：

```text
ChronoSeer/localization/zhs/characters.json
```

至少包括：

- 角色标题；
- 角色描述；
- 自定义模式卡池标题；
- 自定义模式卡池描述；
- 主语/宾语/所有格代词；
- 解锁文本；
- 事件对白；
- 多人模式对白。

---

## 5. 阶段三：制作最小起始卡组

### 5.1 起始卡组建议

最小可玩角色建议先做 5~6 张牌：

| 类型 | 数量 | 示例 |
|---|---:|---|
| 基础攻击 | 4~5 | 打击类 |
| 基础防御 | 4~5 | 防御类 |
| 机制引导牌 | 1 | 生成/消耗核心资源 |
| 初始遗物 | 1 | 让机制有开局支点 |

先不要追求复杂。第一版只要能证明：

- 伤害能造成；
- 格挡能获得；
- 升级能生效；
- 卡图能显示；
- 本地化能显示；
- 卡牌能加入该角色卡池；
- 起始卡组能出现。

### 5.2 创建卡牌基类

建议写一个本角色专用基类，例如：

```text
Scripts/Cards/ChronoCardModel.cs
```

作用：

- 统一卡图路径；
- 统一卡框；
- 统一注册到 `ChronoSeerCardPool`；
- 统一处理缺图占位；
- 后续统一处理角色机制关键词和描述。

### 5.3 创建第一张攻击牌

一张牌至少包含：

- 费用；
- 类型：Attack / Skill / Power；
- 稀有度；
- 目标类型；
- 是否进入图鉴；
- 卡图路径；
- 数值变量；
- `OnPlay` 效果；
- `OnUpgrade` 升级逻辑；
- 本地化名称与描述。

卡牌逻辑建议先参考原版相似卡，再改为你自己的机制。

---

## 6. 阶段四：设计角色核心机制

### 6.1 机制设计文档

在写代码前，先写 `docs/design.md`：

```markdown
# ChronoSeer 角色设计

## 角色主题
时间、预见、延迟结算、回溯、命运标记。

## 核心资源
“刻痕”：由部分卡牌获得，可在回合结束或特定卡牌中消耗。

## 核心循环
获得刻痕 → 延迟伤害/格挡 → 触发预见 → 回收或强化下一回合。

## 风险
当前回合即时收益偏低，需要通过延迟结算换取高收益。

## 卡牌分支
1. 延迟伤害流
2. 预见抽牌流
3. 回溯防御流
4. 稀有爆发流
```

### 6.2 机制落地方式

机制通常通过以下几类代码实现：

- Power：持续状态、回合开始/结束触发；
- DynamicVar：卡牌数值；
- Custom Keyword：自定义关键词；
- Patch：改写原本没有暴露的行为；
- Save Data：跨战斗/局内数据；
- Custom Reward / Custom Target：高级玩法扩展。

建议顺序：

1. 先做一个简单 Power；
2. 让一张卡施加该 Power；
3. 让 Power 在回合结束触发；
4. 加入关键词说明；
5. 加入 UI 或特效；
6. 再拓展复杂效果。

---

## 7. 阶段五：扩充完整卡池

### 7.1 卡池规模建议

完整角色通常可以参考以下规模：

| 稀有度 | 数量建议 |
|---|---:|
| 基础牌 | 5~10 |
| 普通牌 | 25~35 |
| 罕见牌 | 20~25 |
| 稀有牌 | 10~15 |
| 特殊/衍生牌 | 5~10 |
| 总数 | 60~80 |

### 7.2 卡牌表格模板

建议维护 `docs/card_table.md`：

| ID | 中文名 | 类型 | 稀有度 | 费用 | 效果 | 升级 | 分支 | 状态 |
|---|---|---|---|---:|---|---|---|---|
| CHRONO_STRIKE | 时击 | 攻击 | 基础 | 1 | 造成 6 点伤害 | 9 点 | 基础 | 已实现 |
| CHRONO_GUARD | 时盾 | 技能 | 基础 | 1 | 获得 5 点格挡 | 8 点 | 基础 | 已实现 |

### 7.3 平衡原则

每张牌都要检查：

- 是否比原版同费用牌明显超模；
- 升级是否有意义；
- 是否能被力量、敏捷、易伤等系统正确修正；
- 是否有无限循环；
- 是否与遗物/药水产生过强组合；
- 描述是否准确；
- 多人模式是否会出问题。

---

## 8. 阶段六：遗物、药水、能力、关键词

### 8.1 角色初始遗物

初始遗物应该完成两件事：

1. 展示角色核心机制；
2. 限制或引导角色玩法。

例如：

```text
时砂怀表：
战斗开始时获得 1 层“刻痕”。每当你第一次消耗“刻痕”时，抽 1 张牌。
```

### 8.2 角色专属遗物

建议 8~12 个：

- 3 个普通遗物；
- 3 个罕见遗物；
- 2 个稀有遗物；
- 1~2 个事件/特殊遗物。

### 8.3 角色专属药水

建议 2~4 个：

- 一个机制补充型；
- 一个爆发型；
- 一个防御/续航型；
- 一个稀有特殊型。

### 8.4 能力与关键词

至少要有：

- 1 个核心关键词；
- 1~3 个核心 Power；
- 关键词本地化；
- 卡牌描述中的统一写法；
- 图鉴与 tooltip 验证。

---

## 9. 阶段七：原画、卡图与动画

### 9.1 原画制作顺序

建议顺序：

1. 角色剪影；
2. 角色三视图或关键姿态；
3. 战斗待机图；
4. 攻击图；
5. 受击图；
6. 死亡图；
7. 商店/篝火/事件场景图；
8. 角色选择背景；
9. 卡图模板与批量卡图。

### 9.2 风格控制

为了贴近《杀戮尖塔2》风格：

- 先截取游戏内参考画面；
- 用 PureRef 或类似工具整理参考；
- 先保证角色比例与游戏中敌我体量接近；
- 用大色块塑造体积；
- 不要堆过多线条；
- 不要做过强写实光影；
- 服装和发型不要过分复杂，否则后续动画拆分成本会很高。

### 9.3 卡图

新卡牌可以在 `AssetProfile` 中指定：

```csharp
PortraitPath: $"res://ChronoSeer/images/cards/{GetType().Name}.png"
```

建议：

- 普通卡图可先用占位图；
- 文件名与卡牌类名一致；
- 后期批量替换；
- 每张卡导出后在游戏内检查裁切和识别度。

### 9.4 角色动画路线

有两条路线：

#### 路线 A：静态图/帧动画

适合新手，先完成：

- idle；
- hit；
- attack；
- cast；
- dead；
- relaxed。

可用 VisualCueSet / StandardCue 组织动画。  
优点：实现快、资产压力小。  
缺点：表现力不如骨骼动画。

#### 路线 B：Spine 骨骼动画

适合完整高质量角色。  
需要：

- Spine 版本匹配；
- 导出 atlas、skel、png；
- Godot Spine 扩展；
- 战斗模型包含 idle_loop、attack、cast、hurt、die 等动画名；
- 在 Godot 中创建对应资源。

建议：MVP 用路线 A；确定角色机制和卡池稳定后，再升级为 Spine。

---

## 10. 阶段八：事件、怪物、音频与其他内容

完整内容包可以扩展：

- 角色专属事件；
- 角色专属怪物或遭遇；
- 专属音效；
- 专属 VFX；
- 自定义奖励；
- 自定义目标；
- 右键交互；
- 顶栏按钮；
- 局内数据；
- 时间线小故事；
- 模组联动。

优先级建议：

1. 角色机制与卡池；
2. 遗物/药水；
3. 动画与卡图；
4. 事件；
5. 怪物；
6. 音效/VFX；
7. 联动和高级功能。

---

## 11. 阶段九：调试、测试与平衡

### 11.1 日志和控制台

常用检查：

- 游戏是否加载 Mod；
- 日志中是否有初始化输出；
- 控制台能否添加你的卡；
- 卡牌描述是否有缺失 key；
- 图片路径是否存在；
- 战斗中是否报错；
- 保存/读档是否正常。

### 11.2 测试清单

每次新增内容后检查：

- `dotnet build` 通过；
- 游戏启动不报错；
- 角色选择界面正常；
- 进入战斗正常；
- 卡牌能抽到、能打出、能升级；
- 遗物能触发；
- 药水能使用；
- Power 层数显示正确；
- 关键词 tooltip 正确；
- 卡图、遗物图、药水图路径正确；
- 中文/英文文本不缺失；
- 保存读取正常；
- 多人模式字段不会导致异常；
- 没有明显无限循环；
- 没有软锁。

### 11.3 平衡测试

建议记录：

| 测试编号 | 难度 | 卡组流派 | 结果 | 问题 | 调整 |
|---|---|---|---|---|---|
| T001 | A0 | 延迟伤害 | 胜 | 前期过弱 | 加强基础防御 |
| T002 | A3 | 抽牌预见 | 败 | 能量不足 | 加入能量牌 |

---

## 12. 阶段十：打包与发布

### 12.1 本地打包

完整 Mod 文件夹一般包含：

```text
ChronoSeer/
├─ ChronoSeer.dll
├─ ChronoSeer.pck
└─ ChronoSeer.json
```

其中：

- dll：代码；
- pck：Godot 资源包；
- json：Mod 配置；
- 如果依赖 RitsuLib，玩家也要安装对应 RitsuLib。

### 12.2 发布前检查

发布前至少检查：

- ModID 唯一；
- 版本号正确；
- 描述准确；
- 预览图小于平台要求；
- 依赖写清楚；
- 已包含本地化；
- 不包含未授权资产；
- 卡图、音乐、字体、素材版权无问题；
- Changelog 写明；
- 可见性设置正确；
- tags 设置正确；
- 新版游戏兼容性说明。

---

## 13. 推荐的协作方式：你每次应该怎样提示我

为了让我一步步带你制作，你应该把任务拆成“阶段式提示词”，每次只让我做一个阶段。这样最稳。

### 13.1 第一次总控提示词

你可以直接复制下面这段给我：

```text
请作为《杀戮尖塔2》STS2 Modding 技术负责人，基于 tutorials.sts2modding.com 的教程，使用 Godot 4.5.1 .NET + C# + RitsuLib 路线，带我从 0 制作一个完整新角色 Mod。

我的基本信息如下：
1. 操作系统：Windows / Linux / macOS
2. 游戏版本：填写 Slay the Spire 2 当前版本
3. 游戏安装路径：填写路径
4. Godot 版本：填写版本
5. .NET SDK 版本：填写 dotnet --version 输出
6. IDE：Rider / VS Code / Visual Studio
7. ModID：填写英文唯一 ID
8. 角色中文名：
9. 角色英文名：
10. 角色主题：
11. 核心机制关键词：
12. 计划卡牌数量：
13. 美术路线：占位图 / AI 辅助 / 手绘 / Spine
14. 当前阶段：先完成 M0 空 Mod 加载

请你先只输出 M0 的具体文件结构、需要创建的文件、每个文件应写什么、构建命令、验证方法和常见报错排查。不要一次性写完整角色代码。
```

### 13.2 环境配置阶段提示词

```text
我现在要完成 STS2 新角色 Mod 的 M0 阶段：空 Mod 加载。我的环境如下：
- 操作系统：
- Slay the Spire 2 路径：
- 游戏版本：
- Godot 版本：
- dotnet --version：
- IDE：
- ModID：

请你按 RitsuLib 路线给我：
1. Godot 项目创建步骤；
2. csproj 完整模板；
3. mod json 完整模板；
4. Entry.cs 完整代码；
5. dotnet build 后应该看到的目录结构；
6. 游戏内验证方式；
7. 如果失败，按日志如何排查。
```

### 13.3 新角色骨架提示词

```text
我已经完成空 Mod 加载。现在进入 M1：添加新角色骨架。我的 ModID 是【】、角色名是【】、命名空间是【】。请你基于 RitsuLib 生成：
1. CardPool / RelicPool / PotionPool 三个类；
2. Character 类；
3. 起始卡组和起始遗物的最小占位方案；
4. characters.json 本地化模板；
5. 需要放置的图片资源路径；
6. 构建和游戏内验证步骤。
要求先保证角色能出现在角色选择界面并进入第一场战斗。
```

### 13.4 起始卡与第一套机制提示词

```text
现在进入 M2：制作最小可玩机制。角色主题是【】，核心机制是【】。请你帮我设计并实现 10 张 MVP 卡牌：
- 2 张基础攻击；
- 2 张基础防御；
- 3 张机制普通牌；
- 2 张机制罕见牌；
- 1 张机制能力牌。

请先输出卡牌设计表，包括费用、类型、稀有度、效果、升级、机制作用。确认后再生成 C# 代码和 localization/cards.json。
```

### 13.5 美术资产提示词

```text
现在要制作角色美术资产。角色设定如下：【】。请你为《杀戮尖塔2》风格生成一套美术制作清单和提示词，包括：
1. 角色选择立绘；
2. 战斗 idle 图；
3. hit 图；
4. attack 帧动画；
5. cast 帧动画；
6. dead 图；
7. 角色选择背景；
8. 能量图标；
9. 卡框颜色方案；
10. 10 张 MVP 卡图提示词。
要求风格贴近 STS2：美式卡通、色块塑形、线条克制、不过度写实、动作夸张但读图清晰。
```

### 13.6 动画接入提示词

```text
我已经有以下角色图片资源：
- idle：
- hit：
- attack_01：
- attack_02：
- attack_03：
- cast：
- dead：

请你基于 RitsuLib 的 VisualCueSet / StandardCue 帮我接入角色动画，输出：
1. 资源目录；
2. Godot 场景结构；
3. Character 类中 AssetProfile 的写法；
4. SetupCustomCombatAnimationStateMachine 的写法；
5. 攻击/施法延迟如何调；
6. 游戏内验证方法。
```

### 13.7 扩展完整卡池提示词

```text
现在 MVP 已经能运行。请你帮我把角色扩展到完整卡池。角色机制如下：【】；目前已有卡牌如下：【粘贴表格】。请你继续设计：
1. 普通牌补到 30 张；
2. 罕见牌补到 22 张；
3. 稀有牌补到 12 张；
4. 特殊/衍生牌 6 张；
5. 每张牌给出费用、类型、效果、升级、机制分支、强度评估。
要求避免无限循环，保持和原版强度接近，并指出需要重点测试的组合。
```

### 13.8 报错排查提示词

```text
我在制作 STS2 Mod 时遇到报错。请你根据以下信息定位问题：
1. 当前阶段：M0/M1/M2/M3
2. 我刚改了哪些文件：
3. dotnet build 输出：
4. 游戏日志：
5. 控制台报错：
6. 相关代码片段：
7. 资源路径截图或文件树：

请你按“最可能原因 → 验证方法 → 修改方案 → 修改后如何测试”的格式回答。
```

---

## 14. 你现在最应该做的第一步

请先准备并发给我：

```text
我的 STS2 Mod 基础信息：
1. 操作系统：
2. Slay the Spire 2 安装路径：
3. 游戏版本：
4. Godot 版本：
5. dotnet --version：
6. IDE：
7. 想用 RitsuLib 还是 BaseLib：
8. ModID：
9. 角色中文名：
10. 角色英文名：
11. 角色主题一句话：
12. 核心机制一句话：
13. 美术路线：占位图/AI/手绘/Spine
14. 目标：先做 MVP，还是直接做完整内容包？
```

我拿到这些信息后，就可以从 M0 开始，逐个文件带你创建。
