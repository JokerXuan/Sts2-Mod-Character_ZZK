using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Character_ZZK.Scripts.Characters;
using Character_ZZK.Scripts.Powers.Buffs;

namespace Character_ZZK.Scripts.Cards.Rare;

/// <summary>
/// 迷糊时间。
/// 每当生成一张牌时，刻印，并降低该牌费用。
/// </summary>
[RegisterCard(typeof(ZzkCardPool))]
public class MuddledTime : ModCardTemplate
{
    private const int BaseEnergyCost = 2;
    private const CardType CardKind = CardType.Power;
    private const CardRarity CardRareLevel = CardRarity.Rare;
    private const TargetType CardTarget = TargetType.Self;
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://icon.svg"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        ModCardVars.Int("Mark", 3),
        ModCardVars.Int("Reduction", 1)
    ];

    public MuddledTime()
        : base(BaseEnergyCost, CardKind, CardRareLevel, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MuddledTimePower>(
            choiceContext,
            base.Owner.Creature,
            1,
            base.Owner.Creature,
            this
        );

        await PowerCmd.Apply<MuddledTimeMarkPower>(
            choiceContext,
            base.Owner.Creature,
            DynamicVars["Mark"].BaseValue,
            base.Owner.Creature,
            this,
            silent: true
        );

        await PowerCmd.Apply<MuddledTimeCostReductionPower>(
            choiceContext,
            base.Owner.Creature,
            DynamicVars["Reduction"].BaseValue,
            base.Owner.Creature,
            this,
            silent: true
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Mark"].UpgradeValueBy(2);
        DynamicVars["Reduction"].UpgradeValueBy(1);
    }
}