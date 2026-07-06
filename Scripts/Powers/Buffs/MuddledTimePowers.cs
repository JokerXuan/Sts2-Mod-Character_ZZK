using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Character_ZZK.Scripts.Powers.Buffs;

/// <summary>
/// 迷糊时间。
/// 可见 Power，只用于显示“迷糊时间已生效”。
/// 真正的刻印数和减费数值由隐藏 Power 记录。
/// </summary>
[RegisterPower]
public class MuddledTimePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );
    public void FlashSelf(){
        Flash();
    }
}

/// <summary>
/// 迷糊时间的隐藏刻印数记录。
/// Amount 表示每当生成一张牌时刻印多少。
/// </summary>
[RegisterPower]
public class MuddledTimeMarkPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );

    protected override bool IsVisibleInternal => false;

    public override bool ShouldPlayVfx => false;

    public static int GetMarkAmount(Creature creature)
    {
        MuddledTimeMarkPower? power =
            creature.GetPower<MuddledTimeMarkPower>();

        return power == null ? 0 : (int)power.Amount;
    }
}

/// <summary>
/// 迷糊时间的隐藏费用降低记录。
/// Amount 表示生成牌费用降低多少。
/// </summary>
[RegisterPower]
public class MuddledTimeCostReductionPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );

    protected override bool IsVisibleInternal => false;

    public override bool ShouldPlayVfx => false;

    public static int GetReductionAmount(Creature creature)
    {
        MuddledTimeCostReductionPower? power =
            creature.GetPower<MuddledTimeCostReductionPower>();

        return power == null ? 0 : (int)power.Amount;
    }
}