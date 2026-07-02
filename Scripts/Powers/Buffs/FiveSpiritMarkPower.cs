using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Character_ZZK.Scripts.Powers.Buffs;

/// <summary>
/// 灵源印记。
/// 猪猪凯自身积蓄的五灵之力。当前阶段只负责叠层显示，暂不实现消耗与共鸣。
/// </summary>
[RegisterPower]
public class FiveSpiritMarkPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );

    /// <summary>
    /// 刻印：让猪猪凯自己获得灵源印记。
    /// 后续所有“刻印 X”的卡牌都应该调用这个方法。
    /// </summary>
    public static async Task ApplyMark(
        PlayerChoiceContext choiceContext,
        Creature ownerCreature,
        int amount,
        CardModel? cardSource
    )
    {
        if (amount <= 0)
        {
            return;
        }

        if (!ownerCreature.IsAlive)
        {
            return;
        }

        await PowerCmd.Apply<FiveSpiritMarkPower>(
            choiceContext,
            ownerCreature,
            amount,
            ownerCreature,
            cardSource
        );
    }
}