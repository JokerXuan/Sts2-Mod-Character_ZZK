using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Godot;

namespace Character_ZZK.Scripts.Characters;

/// <summary>
/// 猪猪凯角色主体。
/// 这个类负责把“猪猪凯”注册成一个可玩角色，并绑定他的卡牌池、遗物池和药水池。
/// </summary>
[RegisterCharacter]
public class PigKaiCharacter : ModCharacterTemplate<ZzkCardPool, ZzkRelicPool, ZzkPotionPool>
{
    // 角色名称颜色。会影响角色选择界面、部分 UI 文本的颜色。
    public override Color NameColor => new(1.0f, 0.55f, 0.75f);

    // 能量数字描边颜色。和 ZzkCardPool 里的 EnergyOutlineColor 保持接近。
    public override Color EnergyLabelOutlineColor => new(0.95f, 0.35f, 0.55f);

    // 地图绘制颜色。比如地图上和角色相关的标记/绘制颜色。
    public override Color MapDrawingColor => new(1.0f, 0.55f, 0.75f);

    // 角色性别。这里先用 Masculine，因为“猪猪凯”通常按男性称谓处理。
    public override CharacterGender Gender => CharacterGender.Masculine;

    // 初始血量。MVP 阶段先给 75，接近原版角色常见水平。
    public override int StartingHp => 75;

    // 初始金币。先使用常见默认值 99。
    public override int StartingGold => 99;

    // 角色资源配置。
    // M1 阶段我们还没有自己的角色模型、能量表盘、头像、背景，所以先复用 Ironclad 的资源。
    // 后面做美术和动画时，会逐步替换这里。
    public override CharacterAssetProfile AssetProfile => CharacterAssetProfiles.Ironclad();

    // 攻击动画延迟。现在复用原版资源，先设为 0。
    public override float AttackAnimDelay => 0f;

    // 施法动画延迟。现在复用原版资源，先设为 0。
    public override float CastAnimDelay => 0f;

    // 不要求时间线小故事。
    // 如果不加这一句，游戏可能会期待你补时间线相关内容。
    public override bool RequiresEpochAndTimeline => false;

    // 自动从 AssetProfile.Scenes.VisualsPath 创建战斗角色视觉节点。
    // 现在 AssetProfile 复用 Ironclad，所以这里会创建原版 Ironclad 的战斗模型。
    protected override NCreatureVisuals? TryCreateCreatureVisuals()
        => RitsuGodotNodeFactories.CreateFromScenePath<NCreatureVisuals>(AssetProfile.Scenes!.VisualsPath!);

    // 建筑师攻击特效列表。
    // 这里暂时复用原版常见打击特效，后面可以换成猪猪凯自己的特效。
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}