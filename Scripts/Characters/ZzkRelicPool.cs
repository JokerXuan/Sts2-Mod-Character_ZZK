using STS2RitsuLib.Scaffolding.Content;

namespace Character_ZZK.Scripts.Characters;

/// <summary>
/// 猪猪凯的专属遗物池。
/// 后续猪猪凯专属遗物会通过 [RegisterRelic(typeof(ZzkRelicPool))] 注册到这里。
/// </summary>
public class ZzkRelicPool : TypeListRelicPoolModel
{
    // 遗物描述中使用的小能量图标。M1 阶段先用 Godot 默认图标占位。
    public override string? TextEnergyIconPath => "res://icon.svg";

    // tooltip 中使用的大能量图标。M1 阶段先用 Godot 默认图标占位。
    public override string? BigEnergyIconPath => "res://icon.svg";

    // 和 ZzkCardPool 保持一致，代表猪猪凯的能量颜色名。
    public override string EnergyColorName => "zzk";
}