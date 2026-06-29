using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Character_ZZK.Scripts.Powers;

/// <summary>
/// 炽虎形态。
/// M2-1 阶段先作为“形态状态”显示在角色身上。
/// </summary>
[RegisterPower]
public class BlazingTigerFormPower : ModPowerTemplate
{
    // 这是正面能力。
    public override PowerType Type => PowerType.Buff;

    // Single 表示不可重复叠加；重复获得时不会变成多层计数型能力。
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        // 后面替换为：
        // res://Character_ZZK/images/powers/BlazingTigerFormPower.png
        IconPath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );
}