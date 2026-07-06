using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Character_ZZK.Scripts.Characters;
using Character_ZZK.Scripts.Mechanics;

namespace Character_ZZK.Scripts.Cards.Rare;

/// <summary>
/// 五行轮转。
/// 当前形态计数增加，然后触发所有形态共鸣一次。
/// </summary>
[RegisterCard(typeof(ZzkCardPool))]
public class FiveElementsCycle : ModCardTemplate
{
    private const int BaseEnergyCost = 3;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRareLevel = CardRarity.Rare;
    private const TargetType CardTarget = TargetType.Self;
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://icon.svg"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        ModCardVars.Int("Count", 1)
    ];

    public FiveElementsCycle()
        : base(BaseEnergyCost, CardKind, CardRareLevel, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await FiveSpiritResonance.AddCurrentFormResonanceCount(
            choiceContext,
            base.Owner,
            this,
            DynamicVars["Count"].BaseValue
        );

        await FiveSpiritResonance.TriggerAllFormResonanceOnce(
            choiceContext,
            base.Owner,
            this,
            cardPlay
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Count"].UpgradeValueBy(1);
    }
}