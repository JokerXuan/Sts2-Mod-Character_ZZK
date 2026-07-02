using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Character_ZZK.Scripts.Powers.Buffs;

/// <summary>
/// 铁虎共鸣的隐藏清理 Power。
/// 玩家本回合临时获得力量，回合结束时通过这个 Power 扣回去。
/// </summary>
[RegisterPower]
public class IronTigerStrengthCleanupPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );

    protected override bool IsVisibleInternal => false;

    public override bool ShouldPlayVfx => false;

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (!participants.Contains(base.Owner))
        {
            return;
        }

        int amountToRemove = base.Amount;

        if (amountToRemove <= 0)
        {
            await PowerCmd.Remove<IronTigerStrengthCleanupPower>(base.Owner);
            return;
        }

        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            base.Owner,
            -amountToRemove,
            base.Owner,
            null,
            silent: true
        );

        await PowerCmd.Remove<IronTigerStrengthCleanupPower>(base.Owner);
    }
}

/// <summary>
/// 冰鹿共鸣的隐藏清理 Power。
/// 敌人本回合临时失去力量，敌方回合结束时通过这个 Power 还回去。
/// </summary>
[RegisterPower]
public class IceDeerStrengthDownCleanupPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );

    protected override bool IsVisibleInternal => false;

    public override bool ShouldPlayVfx => false;

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (!participants.Contains(base.Owner))
        {
            return;
        }

        int amountToRestore = base.Amount;

        if (amountToRestore <= 0)
        {
            await PowerCmd.Remove<IceDeerStrengthDownCleanupPower>(base.Owner);
            return;
        }

        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            base.Owner,
            amountToRestore,
            base.Owner,
            null,
            silent: true
        );

        await PowerCmd.Remove<IceDeerStrengthDownCleanupPower>(base.Owner);
    }
}