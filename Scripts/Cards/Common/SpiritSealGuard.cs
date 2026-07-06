using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Character_ZZK.Scripts.Characters;
using Character_ZZK.Scripts.Powers.Buffs;

namespace Character_ZZK.Scripts.Cards.Common;

/// <summary>
/// 灵印格挡。
/// 普通技能牌：防御回合也能刻印。
/// </summary>
[RegisterCard(typeof(ZzkCardPool))]
public class SpiritSealGuard : ModCardTemplate
{
    private const int BaseEnergyCost = 2;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRareLevel = CardRarity.Common;
    private const TargetType CardTarget = TargetType.Self;
    private const bool ShowInCardLibrary = true;

    public override bool GainsBlock => true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://icon.svg"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(7, ValueProp.Move),
        ModCardVars.Int("Mark", 2)
    ];

    public SpiritSealGuard()
        : base(BaseEnergyCost, CardKind, CardRareLevel, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(
            base.Owner.Creature,
            DynamicVars.Block,
            cardPlay
        );

        await FiveSpiritMarkPower.ApplyMark(
            choiceContext,
            base.Owner.Creature,
            (int)DynamicVars["Mark"].BaseValue,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
        DynamicVars["Mark"].UpgradeValueBy(2);
    }
}