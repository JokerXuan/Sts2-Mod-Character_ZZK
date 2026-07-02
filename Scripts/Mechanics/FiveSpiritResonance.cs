using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Character_ZZK.Scripts.Powers.Buffs;

namespace Character_ZZK.Scripts.Mechanics;

/// <summary>
/// 五灵共鸣统一逻辑。
/// 打出形态卡后，如果自身灵源印记不少于 2 层，
/// 消耗 2 层灵源印记并触发对应形态的共鸣。
/// </summary>
public static class FiveSpiritResonance
{
    private const int ResonanceCost = 2;

    private static int GetResonanceCount<TCounter>(Creature creature)
        where TCounter : PowerModel
    {
        TCounter? counter = creature.GetPower<TCounter>();
        return counter == null ? 0 : (int)counter.Amount;
    }

    private static async Task<int> TryStartResonance<TCounter>(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource
    )
        where TCounter : PowerModel
    {
        Creature creature = player.Creature;

        bool consumed = await FiveSpiritMarkPower.TryConsumeMark(
            choiceContext,
            creature,
            ResonanceCost,
            cardSource
        );

        if (!consumed)
        {
            return 0;
        }

        int nextCount = GetResonanceCount<TCounter>(creature) + 1;

        await PowerCmd.Apply<TCounter>(
            choiceContext,
            creature,
            1m,
            creature,
            cardSource,
            silent: true
        );

        return nextCount;
    }

    /// <summary>
    /// 铁虎共鸣：
    /// 本回合获得 X 点力量。
    /// X = 本场战斗铁虎共鸣次数。
    /// </summary>
    public static async Task TriggerIronTiger(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource
    )
    {
        int value = await TryStartResonance<IronTigerResonanceCountPower>(
            choiceContext,
            player,
            cardSource
        );

        if (value <= 0)
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(
        choiceContext,
        player.Creature,
        value,
        player.Creature,
        cardSource
    );

    await PowerCmd.Apply<IronTigerStrengthCleanupPower>(
        choiceContext,
        player.Creature,
        value,
        player.Creature,
        cardSource,
        silent: true
    );
    }

    /// <summary>
    /// 木猿共鸣：
    /// 本局永久获得 1 点能量上限，并抽 1 张牌。
    /// </summary>
    public static async Task TriggerWoodApe(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource
    )
    {
        int value = await TryStartResonance<WoodApeResonanceCountPower>(
            choiceContext,
            player,
            cardSource
        );

        if (value <= 0)
        {
            return;
        }

        await CardPileCmd.Draw(
            choiceContext,
            player
        );
    }

    /// <summary>
    /// 冰鹿共鸣：
    /// 所有敌人在本回合失去 X 点力量。
    /// X = 本场战斗冰鹿共鸣次数。
    /// </summary>
    public static async Task TriggerIceDeer(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource
    )
    {
        int value = await TryStartResonance<IceDeerResonanceCountPower>(
            choiceContext,
            player,
            cardSource
        );

        if (value <= 0)
        {
            return;
        }

        ICombatState? combatState = player.Creature.CombatState;

        if (combatState == null || !combatState.IsLiveCombat())
        {
            return;
        }

        var enemies = combatState.HittableEnemies
            .Where(enemy => enemy.IsAlive)
            .ToList();

        if (enemies.Count == 0)
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(
        choiceContext,
        enemies,
        -value,
        player.Creature,
        cardSource
    );

    await PowerCmd.Apply<IceDeerStrengthDownCleanupPower>(
        choiceContext,
        enemies,
        value,
        player.Creature,
        cardSource,
        silent: true
    );
    }

    /// <summary>
    /// 火鹤共鸣：
    /// 对所有敌人造成 X 点伤害，重复 Y 次。
    /// X = 本场战斗火鹤共鸣次数。
    /// Y = 当前剩余能量。
    /// </summary>
    public static async Task TriggerFireCrane(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource
    )
    {
        int damagePerHit = await TryStartResonance<FireCraneResonanceCountPower>(
            choiceContext,
            player,
            cardSource
        );

        if (damagePerHit <= 0)
        {
            return;
        }

        ICombatState? combatState = player.Creature.CombatState;

        if (combatState == null || !combatState.IsLiveCombat())
        {
            return;
        }

        int hitTimes = player.PlayerCombatState?.Energy ?? 0;

        if (hitTimes <= 0)
        {
            return;
        }

        for (int i = 0; i < hitTimes; i++)
        {
            var enemies = combatState.HittableEnemies
                .Where(enemy => enemy.IsAlive)
                .ToList();

            if (enemies.Count == 0)
            {
                return;
            }

            await CreatureCmd.Damage(
                choiceContext,
                enemies,
                damagePerHit,
                ValueProp.Unpowered,
                player.Creature,
                cardSource
            );
        }
    }

    /// <summary>
    /// 石熊共鸣：
    /// 获得 X 层覆甲。
    /// X = 本场战斗石熊共鸣次数。
    /// </summary>
    public static async Task TriggerStoneBear(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource
    )
    {
        int value = await TryStartResonance<StoneBearResonanceCountPower>(
            choiceContext,
            player,
            cardSource
        );

        if (value <= 0)
        {
            return;
        }

        await PowerCmd.Apply<PlatingPower>(
            choiceContext,
            player.Creature,
            value,
            player.Creature,
            cardSource
        );
    }
}