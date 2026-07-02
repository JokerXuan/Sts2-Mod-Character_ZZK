using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Character_ZZK.Scripts.Powers.Buffs;

/// <summary>
/// 五灵共鸣次数记录基类。
/// 只用于本场战斗内计数。
/// </summary>
public abstract class FiveSpiritResonanceCounterPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );
}

[RegisterPower]
public class IronTigerResonanceCountPower : FiveSpiritResonanceCounterPower
{
}

[RegisterPower]
public class WoodApeResonanceCountPower : FiveSpiritResonanceCounterPower
{
    /// <summary>
    /// 木猿共鸣：本场战斗内提高能量上限。
    /// Amount 等于本场战斗触发木猿共鸣的次数。
    /// </summary>
    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        if (player.Creature != base.Owner)
        {
            return amount;
        }

        return amount + base.Amount;
    }
}

[RegisterPower]
public class IceDeerResonanceCountPower : FiveSpiritResonanceCounterPower
{
}

[RegisterPower]
public class FireCraneResonanceCountPower : FiveSpiritResonanceCounterPower
{
}

[RegisterPower]
public class StoneBearResonanceCountPower : FiveSpiritResonanceCounterPower
{
}