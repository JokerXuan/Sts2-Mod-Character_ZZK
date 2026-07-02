using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Character_ZZK.Scripts.Characters;
using Character_ZZK.Scripts.Mechanics;
using Character_ZZK.Scripts.Powers.Buffs;

namespace Character_ZZK.Scripts.Relics.Starter;

/// <summary>
/// 猪猪凯的初始遗物：五灵锁。
/// 每回合抽牌前，如果手牌中没有五灵觉醒牌，
/// 则随机生成一张当前未处于对应形态的五灵觉醒牌。
/// 战斗第一回合额外获得 1 层灵源印记。
/// </summary>
[RegisterRelic(typeof(ZzkRelicPool))]
[RegisterCharacterStarterRelic(typeof(PigKaiCharacter))]
public class FiveSpiritLock : ModRelicTemplate
{
    private const int StartingMarkAmount = 1;

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: "res://icon.svg",
        IconOutlinePath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState
    )
    {
        if (player != base.Owner)
        {
            return;
        }

        if (!combatState.IsLiveCombat())
        {
            return;
        }

        bool hasFlashed = false;

        if (combatState.RoundNumber == 1)
        {
            Flash();
            hasFlashed = true;

            await FiveSpiritMarkPower.ApplyMark(
                choiceContext,
                base.Owner.Creature,
                StartingMarkAmount,
                null
            );
        }

        if (FiveSpiritFormUtils.HasAnyAwakeningInHand(base.Owner))
        {
            return;
        }

        var options = FiveSpiritFormUtils.GetAvailableFormOptions(base.Owner);

        if (options.Count == 0)
        {
            return;
        }

        FiveSpiritFormOption selected =
            combatState.RunState.Rng.CombatCardGeneration.NextItem(options);

        if (!hasFlashed)
        {
            Flash();
        }

        await selected.CreateInHand(base.Owner, combatState);
    }
}