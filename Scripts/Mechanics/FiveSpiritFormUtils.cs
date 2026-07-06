using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Character_ZZK.Scripts.Cards.Special;
using Character_ZZK.Scripts.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Character_ZZK.Scripts.Mechanics;

public readonly record struct FiveSpiritFormOption(
    string Id,
    Func<Player, PlayerChoiceContext, ICombatState, Task> CreateInHand
);

public static class FiveSpiritFormUtils
{
    public static bool IsFiveSpiritAwakening(CardModel card)
    {
        return card is IronTigerAwakening
            or WoodApeAwakening
            or IceDeerAwakening
            or FireCraneAwakening
            or StoneBearAwakening;
    }

    public static bool HasAnyAwakeningInHand(Player player)
    {
        return PileType.Hand
            .GetPile(player)
            .Cards
            .Any(IsFiveSpiritAwakening);
    }

    public static bool ReduceAwakeningCostsInHand(Player player)
    {
        bool reducedAny = false;

        foreach (CardModel card in PileType.Hand.GetPile(player).Cards)
        {
            if (!IsFiveSpiritAwakening(card))
            {
                continue;
            }

            card.EnergyCost.AddUntilPlayed(
                -1,
                reduceOnly: true
            );

            reducedAny = true;
        }

        return reducedAny;
    }

    public static List<FiveSpiritFormOption> GetAvailableFormOptions(Player player)
    {
        Creature creature = player.Creature;
        List<FiveSpiritFormOption> options = [];

        if (!creature.HasPower<IronTigerFormPower>())
        {
            options.Add(new FiveSpiritFormOption("IronTiger", IronTigerAwakening.CreateInHand));
        }

        if (!creature.HasPower<WoodApeFormPower>())
        {
            options.Add(new FiveSpiritFormOption("WoodApe", WoodApeAwakening.CreateInHand));
        }

        if (!creature.HasPower<IceDeerFormPower>())
        {
            options.Add(new FiveSpiritFormOption("IceDeer", IceDeerAwakening.CreateInHand));
        }

        if (!creature.HasPower<FireCraneFormPower>())
        {
            options.Add(new FiveSpiritFormOption("FireCrane", FireCraneAwakening.CreateInHand));
        }

        if (!creature.HasPower<StoneBearFormPower>())
        {
            options.Add(new FiveSpiritFormOption("StoneBear", StoneBearAwakening.CreateInHand));
        }

        return options;
    }

    public static async Task RemoveOtherForms<TCurrent>(Creature creature)
        where TCurrent : PowerModel
    {
        if (typeof(TCurrent) != typeof(IronTigerFormPower))
        {
            await PowerCmd.Remove<IronTigerFormPower>(creature);
        }

        if (typeof(TCurrent) != typeof(WoodApeFormPower))
        {
            await PowerCmd.Remove<WoodApeFormPower>(creature);
        }

        if (typeof(TCurrent) != typeof(IceDeerFormPower))
        {
            await PowerCmd.Remove<IceDeerFormPower>(creature);
        }

        if (typeof(TCurrent) != typeof(FireCraneFormPower))
        {
            await PowerCmd.Remove<FireCraneFormPower>(creature);
        }

        if (typeof(TCurrent) != typeof(StoneBearFormPower))
        {
            await PowerCmd.Remove<StoneBearFormPower>(creature);
        }
    }

    public static async Task SwitchToIronTigerForm(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel sourceCard
    )
    {
        await RemoveOtherForms<IronTigerFormPower>(player.Creature);

        await PowerCmd.Apply<IronTigerFormPower>(
            choiceContext,
            player.Creature,
            1m,
            player.Creature,
            sourceCard
        );
    }

    public static async Task SwitchToWoodApeForm(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel sourceCard
    )
    {
        await RemoveOtherForms<WoodApeFormPower>(player.Creature);

        await PowerCmd.Apply<WoodApeFormPower>(
            choiceContext,
            player.Creature,
            1m,
            player.Creature,
            sourceCard
        );
    }

    public static async Task SwitchToIceDeerForm(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel sourceCard
    )
    {
        await RemoveOtherForms<IceDeerFormPower>(player.Creature);

        await PowerCmd.Apply<IceDeerFormPower>(
            choiceContext,
            player.Creature,
            1m,
            player.Creature,
            sourceCard
        );
    }

    public static async Task SwitchToFireCraneForm(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel sourceCard
    )
    {
        await RemoveOtherForms<FireCraneFormPower>(player.Creature);

        await PowerCmd.Apply<FireCraneFormPower>(
            choiceContext,
            player.Creature,
            1m,
            player.Creature,
            sourceCard
        );
    }

    public static async Task SwitchToStoneBearForm(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel sourceCard
    )
    {
        await RemoveOtherForms<StoneBearFormPower>(player.Creature);

        await PowerCmd.Apply<StoneBearFormPower>(
            choiceContext,
            player.Creature,
            1m,
            player.Creature,
            sourceCard
        );
    }

    public static async Task SwitchToFormByAwakeningCard(
        PlayerChoiceContext choiceContext,
        Player player,
        CardModel selectedCard,
        CardModel sourceCard
    )
    {
        if (selectedCard is IronTigerAwakening)
        {
            await SwitchToIronTigerForm(choiceContext, player, sourceCard);
            return;
        }

        if (selectedCard is WoodApeAwakening)
        {
            await SwitchToWoodApeForm(choiceContext, player, sourceCard);
            return;
        }

        if (selectedCard is IceDeerAwakening)
        {
            await SwitchToIceDeerForm(choiceContext, player, sourceCard);
            return;
        }

        if (selectedCard is FireCraneAwakening)
        {
            await SwitchToFireCraneForm(choiceContext, player, sourceCard);
            return;
        }

        if (selectedCard is StoneBearAwakening)
        {
            await SwitchToStoneBearForm(choiceContext, player, sourceCard);
        }
    }
}