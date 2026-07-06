using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Character_ZZK.Scripts.Characters;

namespace Character_ZZK.Scripts.Cards.Uncommon;

/// <summary>
/// 五灵共鸣。
/// 未升级：触发随机形态共鸣。
/// 升级后：触发当前形态共鸣。
/// </summary>
[RegisterCard(typeof(ZzkCardPool))]
public class FiveSpiritResonanceSkill : ModCardTemplate
{
    private const int BaseEnergyCost = 2;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRareLevel = CardRarity.Uncommon;
    private const TargetType CardTarget = TargetType.Self;
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://icon.svg"
    );

    public FiveSpiritResonanceSkill()
        : base(BaseEnergyCost, CardKind, CardRareLevel, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsUpgraded)
        {
            await Character_ZZK.Scripts.Mechanics.FiveSpiritResonance
                .TriggerCurrentFormResonance(
                    choiceContext,
                    base.Owner,
                    this,
                    cardPlay
                );

            return;
        }

        await Character_ZZK.Scripts.Mechanics.FiveSpiritResonance
            .TriggerRandomFormResonance(
                choiceContext,
                base.Owner,
                this,
                cardPlay
            );
    }
}