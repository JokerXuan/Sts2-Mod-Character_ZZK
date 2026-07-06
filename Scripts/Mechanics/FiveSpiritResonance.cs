using System;
using System.Collections.Generic;
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
using Character_ZZK.Scripts.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models.Cards;

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
    /// 触发随机一种形态的共鸣。
    /// 不切换形态，只执行该形态的共鸣效果。
    /// </summary>
    public static async Task TriggerRandomFormResonance(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource,
        CardPlay cardPlay
    )
    {
        ICombatState? combatState = player.Creature.CombatState;

        if (combatState == null || !combatState.IsLiveCombat())
        {
            return;
        }

        List<Func<Task>> options = [
            () => TriggerIronTiger(choiceContext, player, cardSource),
            () => TriggerWoodApe(choiceContext, player, cardSource),
            () => TriggerIceDeer(choiceContext, player, cardSource),
            () => TriggerFireCrane(choiceContext, player, cardSource, cardPlay),
            () => TriggerStoneBear(choiceContext, player, cardSource)
        ];

        Func<Task>? selected =
            combatState.RunState.Rng.CombatCardGeneration.NextItem(options);

        if (selected == null)
        {
            return;
        }

        await selected();
    }

    /// <summary>
    /// 触发当前形态的共鸣。
    /// 如果当前没有五灵形态，则随机触发一种形态共鸣。
    /// </summary>
    public static async Task TriggerCurrentFormResonance(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource,
        CardPlay cardPlay
    )
    {
        Creature creature = player.Creature;

        if (creature.HasPower<IronTigerFormPower>())
        {
            await TriggerIronTiger(choiceContext, player, cardSource);
            return;
        }

        if (creature.HasPower<WoodApeFormPower>())
        {
            await TriggerWoodApe(choiceContext, player, cardSource);
            return;
        }

        if (creature.HasPower<IceDeerFormPower>())
        {
            await TriggerIceDeer(choiceContext, player, cardSource);
            return;
        }

        if (creature.HasPower<FireCraneFormPower>())
        {
            await TriggerFireCrane(choiceContext, player, cardSource, cardPlay);
            return;
        }

        if (creature.HasPower<StoneBearFormPower>())
        {
            await TriggerStoneBear(choiceContext, player, cardSource);
            return;
        }

        await TriggerRandomFormResonance(
            choiceContext,
            player,
            cardSource,
            cardPlay
        );
    }

    private static bool CanIceDeerGenerate(CardModel card)
    {
        if (!card.ShouldShowInCardLibrary)
        {
            return false;
        }

        if (!card.CanBeGeneratedInCombat)
        {
            return false;
        }

        if (card.Type is not (CardType.Attack or CardType.Skill or CardType.Power))
        {
            return false;
        }

        if (card.Rarity is not (CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare))
        {
            return false;
        }

        return true;
    }

    private static async Task AddRandomIceDeerGeneratedCardToHand(
        PlayerChoiceContext choiceContext,
        Player player,
        ICombatState combatState,
        CardModel cardSource
    )
    {
        List<CardModel> candidates = ModelDb.AllCards
            .Where(CanIceDeerGenerate)
            .ToList();

        if (candidates.Count == 0)
        {
            return;
        }

        CardModel? canonicalCard =
            combatState.RunState.Rng.CombatCardGeneration.NextItem(candidates);

        if (canonicalCard == null)
        {
            return;
        }

        CardModel generatedCard =
            combatState.CreateCard(canonicalCard, player);

        generatedCard.AddKeyword(CardKeyword.Ethereal);
        generatedCard.AddKeyword(CardKeyword.Exhaust);

        await GeneratedCardEffectUtils.ApplyOnGeneratedCardEffects(
            choiceContext,
            player,
            generatedCard,
            cardSource
        );

        await CardPileCmd.AddGeneratedCardToCombat(
            generatedCard,
            PileType.Hand,
            player
        );
    }

    /// <summary>
    /// 当前形态的共鸣计数增加指定层数。
    /// 如果当前没有五灵形态，则不增加任何计数。
    /// 这里只增加计数，不触发该形态的共鸣效果，也不消耗灵源印记。
    /// </summary>
    public static async Task AddCurrentFormResonanceCount(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource,
        decimal amount
    )
    {
        if (amount <= 0)
        {
            return;
        }

        Creature creature = player.Creature;

        if (creature.HasPower<IronTigerFormPower>())
        {
            await PowerCmd.Apply<IronTigerResonanceCountPower>(
                choiceContext,
                creature,
                amount,
                creature,
                cardSource,
                silent: true
            );
            return;
        }

        if (creature.HasPower<WoodApeFormPower>())
        {
            await PowerCmd.Apply<WoodApeResonanceCountPower>(
                choiceContext,
                creature,
                amount,
                creature,
                cardSource,
                silent: true
            );
            return;
        }

        if (creature.HasPower<IceDeerFormPower>())
        {
            await PowerCmd.Apply<IceDeerResonanceCountPower>(
                choiceContext,
                creature,
                amount,
                creature,
                cardSource,
                silent: true
            );
            return;
        }

        if (creature.HasPower<FireCraneFormPower>())
        {
            await PowerCmd.Apply<FireCraneResonanceCountPower>(
                choiceContext,
                creature,
                amount,
                creature,
                cardSource,
                silent: true
            );
            return;
        }

        if (creature.HasPower<StoneBearFormPower>())
        {
            await PowerCmd.Apply<StoneBearResonanceCountPower>(
                choiceContext,
                creature,
                amount,
                creature,
                cardSource,
                silent: true
            );
        }
    }

    /// <summary>
    /// 依次触发五种形态的共鸣一次。
    /// 每一种共鸣都会走 TryStartResonance，因此每次都需要消耗 2 层灵源印记。
    /// </summary>
    public static async Task TriggerAllFormResonanceOnce(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel cardSource,
        CardPlay cardPlay
    )
    {
        await TriggerIronTiger(
            choiceContext,
            player,
            cardSource
        );

        await TriggerWoodApe(
            choiceContext,
            player,
            cardSource
        );

        await TriggerIceDeer(
            choiceContext,
            player,
            cardSource
        );

        await TriggerFireCrane(
            choiceContext,
            player,
            cardSource,
            cardPlay
        );

        await TriggerStoneBear(
            choiceContext,
            player,
            cardSource
        );
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
    /// 随机从所有可生成卡池中生成 1 张牌加入手牌，该牌获得虚无和消耗。
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

        if (enemies.Count > 0)
        {
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

        await AddRandomIceDeerGeneratedCardToHand(
            choiceContext,
            player,
            combatState,
            cardSource
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
        CardModel cardSource,
        CardPlay cardPlay
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

        await DamageCmd.Attack(damagePerHit)
            .FromCard(cardSource, cardPlay)
            .TargetingAllOpponents(combatState)
            .WithHitCount(hitTimes)
            .Unpowered()
            .WithNoAttackerAnim()
            .Execute(choiceContext);
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