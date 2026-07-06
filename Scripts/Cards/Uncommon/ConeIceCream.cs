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

namespace Character_ZZK.Scripts.Cards.Uncommon;

/// <summary>
/// 甜筒冰淇淋。
/// 0费刻印、抽牌、消耗。
/// </summary>
[RegisterCard(typeof(ZzkCardPool))]
public class ConeIceCream : ModCardTemplate
{
    private const int BaseEnergyCost = 0;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRareLevel = CardRarity.Uncommon;
    private const TargetType CardTarget = TargetType.Self;
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://icon.svg"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        ModCardVars.Int("Mark", 2),
        ModCardVars.Int("Draw", 1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    public ConeIceCream()
        : base(BaseEnergyCost, CardKind, CardRareLevel, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await FiveSpiritMarkPower.ApplyMark(
            choiceContext,
            base.Owner.Creature,
            (int)DynamicVars["Mark"].BaseValue,
            this
        );

        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars["Draw"].BaseValue,
            base.Owner
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Mark"].UpgradeValueBy(1);
        DynamicVars["Draw"].UpgradeValueBy(1);
    }
}