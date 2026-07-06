using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Character_ZZK.Scripts.Powers.Buffs;

namespace Character_ZZK.Scripts.Mechanics;

/// <summary>
/// 统一处理“生成卡牌后”的附加效果。
/// 目前用于迷糊时间。
/// </summary>
public static class GeneratedCardEffectUtils
{
    public static async Task ApplyOnGeneratedCardEffects(
        PlayerChoiceContext choiceContext,
        Player owner,
        CardModel generatedCard,
        CardModel? sourceCard
    )
    {
        Creature creature = owner.Creature;

        int markAmount =
            MuddledTimeMarkPower.GetMarkAmount(creature);

        int reductionAmount =
            MuddledTimeCostReductionPower.GetReductionAmount(creature);

        if (markAmount <= 0 && reductionAmount <= 0)
        {
            return;
        }

        MuddledTimePower? muddledTimePower =
            creature.GetPower<MuddledTimePower>();

        muddledTimePower?.FlashSelf();

        if (markAmount > 0)
        {
            await FiveSpiritMarkPower.ApplyMark(
                choiceContext,
                creature,
                markAmount,
                sourceCard
            );
        }

        if (reductionAmount > 0)
        {
            generatedCard.EnergyCost.AddUntilPlayed(
                -reductionAmount,
                reduceOnly: true
            );
        }
    }
}