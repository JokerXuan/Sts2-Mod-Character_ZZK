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
/// 猪猪凯自身积蓄的五灵之力。
/// 当前阶段：可以刻印、查询层数、被共鸣消耗。
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

        int bonusAmount = SpiralSpiritCorePower.GetBonusMarkAmount(ownerCreature);
        int finalAmount = amount + bonusAmount;

        await PowerCmd.Apply<FiveSpiritMarkPower>(
            choiceContext,
            ownerCreature,
            finalAmount,
            ownerCreature,
            cardSource
        );
    }

    /// <summary>
    /// 读取当前灵源印记层数。
    /// </summary>
    public static int GetMarkAmount(Creature ownerCreature)
    {
        FiveSpiritMarkPower? markPower =
            ownerCreature.GetPower<FiveSpiritMarkPower>();

        return markPower == null ? 0 : (int)markPower.Amount;
    }

    /// <summary>
    /// 尝试消耗指定层数的灵源印记。
    /// 如果层数不足，返回 false。
    /// </summary>
    public static async Task<bool> TryConsumeMark(
        PlayerChoiceContext choiceContext,
        Creature ownerCreature,
        int amount,
        CardModel? cardSource
    )
    {
        if (amount <= 0)
        {
            return true;
        }

        FiveSpiritMarkPower? markPower =
            ownerCreature.GetPower<FiveSpiritMarkPower>();

        if (markPower == null)
        {
            return false;
        }

        if (markPower.Amount < amount)
        {
            return false;
        }

        await PowerCmd.ModifyAmount(
            choiceContext,
            markPower,
            -amount,
            ownerCreature,
            cardSource
        );

        return true;
    }
}