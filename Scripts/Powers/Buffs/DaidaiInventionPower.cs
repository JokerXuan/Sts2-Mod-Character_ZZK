using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Character_ZZK.Scripts.Powers.Buffs;

/// <summary>
/// 呆呆发明。
/// 每回合开始时，刻印等同于本 Power 层数的灵源印记。
/// </summary>
[RegisterPower]
public class DaidaiInventionPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        if (player != base.Owner.Player)
        {
            return;
        }

        if (!base.Owner.IsAlive)
        {
            return;
        }

        Flash();

        await FiveSpiritMarkPower.ApplyMark(
            choiceContext,
            base.Owner,
            (int)base.Amount,
            null
        );
    }
}