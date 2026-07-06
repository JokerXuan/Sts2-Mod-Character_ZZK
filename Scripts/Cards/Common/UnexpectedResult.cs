using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Character_ZZK.Scripts.Characters;
using Character_ZZK.Scripts.Powers.Buffs;

namespace Character_ZZK.Scripts.Cards.Common;

/// <summary>
/// 意外成果。
/// 随机消耗一张手牌，并获得大量灵源印记。
/// </summary>
[RegisterCard(typeof(ZzkCardPool))]
public class UnexpectedResult : ModCardTemplate
{
    private const int BaseEnergyCost = 1;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRareLevel = CardRarity.Common;
    private const TargetType CardTarget = TargetType.Self;
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://icon.svg"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        ModCardVars.Int("Mark", 5)
    ];

    public UnexpectedResult()
        : base(BaseEnergyCost, CardKind, CardRareLevel, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ICombatState? combatState = base.Owner.Creature.CombatState;

        if (combatState != null && combatState.IsLiveCombat())
        {
            List<CardModel> handCards = PileType.Hand
                .GetPile(base.Owner)
                .Cards
                .ToList();

            if (handCards.Count > 0)
            {
                CardModel? selectedCard =
                    combatState.RunState.Rng.CombatCardGeneration.NextItem(handCards);

                if (selectedCard != null)
                {
                    await CardPileCmd.Add(
                        selectedCard,
                        PileType.Exhaust
                    );
                }
            }
        }

        await FiveSpiritMarkPower.ApplyMark(
            choiceContext,
            base.Owner.Creature,
            (int)DynamicVars["Mark"].BaseValue,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Mark"].UpgradeValueBy(3);
    }
}