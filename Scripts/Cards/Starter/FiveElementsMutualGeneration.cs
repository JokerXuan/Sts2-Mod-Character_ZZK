using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Character_ZZK.Scripts.Cards.Special;
using Character_ZZK.Scripts.Characters;
using Character_ZZK.Scripts.Mechanics;
using System.Linq;
using MegaCrit.Sts2.Core.CardSelection;

namespace Character_ZZK.Scripts.Cards.Starter;

/// <summary>
/// 五行相生。
/// 选择一种五灵形态并切换，不触发共鸣。
/// </summary>
[RegisterCard(typeof(ZzkCardPool))]
[RegisterCharacterStarterCard(typeof(PigKaiCharacter), 1)]
public class FiveElementsMutualGeneration : ModCardTemplate
{
    private const int BaseEnergyCost = 1;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRareLevel = CardRarity.Common;
    private const TargetType CardTarget = TargetType.Self;
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://icon.svg"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public FiveElementsMutualGeneration()
        : base(BaseEnergyCost, CardKind, CardRareLevel, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ICombatState? combatState = base.Owner.Creature.CombatState;

        if (combatState == null || !combatState.IsLiveCombat())
        {
            return;
        }

        await CreatureCmd.TriggerAnim(
            base.Owner.Creature,
            "Cast",
            base.Owner.Character.CastAnimDelay
        );

        List<CardModel> choices = [
            combatState.CreateCard<IronTigerAwakening>(base.Owner),
            combatState.CreateCard<WoodApeAwakening>(base.Owner),
            combatState.CreateCard<IceDeerAwakening>(base.Owner),
            combatState.CreateCard<FireCraneAwakening>(base.Owner),
            combatState.CreateCard<StoneBearAwakening>(base.Owner)
        ];

        CardSelectorPrefs prefs = new(
            base.SelectionScreenPrompt,
            1
        );

        IEnumerable<CardModel> selectedCards =
        await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            choices,
            base.Owner,
            prefs
        );

        CardModel? selectedCard = selectedCards.FirstOrDefault();

        if (selectedCard == null)
        {
            return;
        }

        await FiveSpiritFormUtils.SwitchToFormByAwakeningCard(
            choiceContext,
            base.Owner,
            selectedCard,
            this
        );
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}