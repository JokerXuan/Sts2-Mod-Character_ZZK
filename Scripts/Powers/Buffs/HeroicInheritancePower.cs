using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Character_ZZK.Scripts.Powers.Buffs;

/// <summary>
/// 英雄继承。
/// 本回合接下来的时间中，每打出一张牌，刻印等同于本 Power 层数的灵源印记。
/// </summary>
[RegisterPower]
public class HeroicInheritancePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay
    )
    {
        if (cardPlay.Card.Owner.Creature != base.Owner)
        {
            return;
        }

        // 英雄继承自身不触发自己的“接下来的时间”效果。
        if (cardPlay.Card is Character_ZZK.Scripts.Cards.Uncommon.HeroicInheritance)
        {
            return;
        }

        await FiveSpiritMarkPower.ApplyMark(
            choiceContext,
            base.Owner,
            (int)base.Amount,
            cardPlay.Card
        );
    }

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

        await PowerCmd.Remove<HeroicInheritancePower>(base.Owner);
    }
}