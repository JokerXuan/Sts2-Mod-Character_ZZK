using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Character_ZZK.Scripts.Powers.Buffs;

/// <summary>
/// 旋灵核。
/// 每次刻印时，额外获得等同于本 Power 层数的灵源印记。
/// </summary>
[RegisterPower]
public class SpiralSpiritCorePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );

    public static int GetBonusMarkAmount(Creature creature)
    {
        SpiralSpiritCorePower? power = creature.GetPower<SpiralSpiritCorePower>();

        return power == null ? 0 : (int)power.Amount;
    }
}