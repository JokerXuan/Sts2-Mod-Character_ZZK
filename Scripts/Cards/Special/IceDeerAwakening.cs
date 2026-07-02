using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Character_ZZK.Scripts.Characters;
using Character_ZZK.Scripts.Mechanics;
using Character_ZZK.Scripts.Powers;

namespace Character_ZZK.Scripts.Cards.Special;

[RegisterCard(typeof(ZzkCardPool))]
public class IceDeerAwakening : ModCardTemplate
{
    private const int BaseEnergyCost = 2;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRareLevel = CardRarity.Token;
    private const TargetType CardTarget = TargetType.Self;
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(PortraitPath: "res://icon.svg");

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust,
        CardKeyword.Retain
    ];

    public override bool CanBeGeneratedInCombat => false;
    public override bool CanBeGeneratedByModifiers => false;

    public IceDeerAwakening()
        : base(BaseEnergyCost, CardKind, CardRareLevel, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            base.Owner.Creature,
            "Cast",
            base.Owner.Character.CastAnimDelay
        );

        await FiveSpiritFormUtils.RemoveOtherForms<IceDeerFormPower>(
            base.Owner.Creature
        );

        await PowerCmd.Apply<IceDeerFormPower>(
            choiceContext,
            base.Owner.Creature,
            1m,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
    }

    public static async Task CreateInHand(Player owner, ICombatState combatState)
    {
        if (!combatState.IsLiveCombat())
        {
            return;
        }

        IceDeerAwakening card = combatState.CreateCard<IceDeerAwakening>(owner);

        await CardPileCmd.AddGeneratedCardToCombat(
            card,
            PileType.Hand,
            owner
        );
    }
}