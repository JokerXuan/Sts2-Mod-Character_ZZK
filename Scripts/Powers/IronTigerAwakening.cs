using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Character_ZZK.Scripts.Powers;

/// <summary>
/// 铁虎形态。
/// 当前阶段先作为五灵形态状态显示。
/// 后续可以扩展为：攻击牌触发追击，生成“铁拳”Token。
/// </summary>
[RegisterPower]
public class IronTigerFormPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );
}