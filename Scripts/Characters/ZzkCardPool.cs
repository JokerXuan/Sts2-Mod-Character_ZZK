using Godot;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace Character_ZZK.Scripts.Characters;

/// <summary>
/// 猪猪凯的专属卡牌池。
/// 控制猪猪凯卡牌池、能量图标、卡框颜色等视觉风格。
/// </summary>
public class ZzkCardPool : TypeListCardPoolModel
{
    public override string Title => "zzk";

    public override string EnergyColorName => "zzk";

    public override string? TextEnergyIconPath => "res://icon.svg";

    public override string? BigEnergyIconPath => "res://icon.svg";

    // 非常亮的柠檬黄：强辨识度，和目前尝试过的黑、粉、紫、橙、棕、青都明显不同。
    public override Color DeckEntryCardColor => new(1.0f, 0.92f, 0.05f);

    // 深琥珀描边：避免亮黄色在 UI 中发白，同时保留“金黄”质感。
    public override Color EnergyOutlineColor => new(0.42f, 0.24f, 0.02f);

    private static readonly Material? _poolFrameMaterial =
        MaterialUtils.CreateReplaceHueShaderMaterial(1.0f, 0.92f, 0.05f);

    public override Material? PoolFrameMaterial => _poolFrameMaterial;

    public override bool IsColorless => false;
}