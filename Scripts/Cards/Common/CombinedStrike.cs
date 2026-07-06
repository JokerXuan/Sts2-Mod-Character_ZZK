using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Character_ZZK.Scripts.Characters;
using Character_ZZK.Scripts.Powers.Buffs;

namespace Character_ZZK.Scripts.Cards.Common;

/// <summary>
/// 合击。
/// 造成五种形态共鸣次数之和的伤害，并刻印相应数量。
/// 升级后：刻印数 +1。
/// </summary>
[RegisterCard(typeof(ZzkCardPool))]
public class CombinedStrike : ModCardTemplate
{
    private const int BaseEnergyCost = 1;
    private const CardType CardKind = CardType.Attack;
    private const CardRarity CardRareLevel = CardRarity.Common;
    private const TargetType CardTarget = TargetType.AnyEnemy;
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://icon.svg"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ResonanceTotalVar(),
        new ComboMarkVar()
    ];

    public CombinedStrike()
        : base(BaseEnergyCost, CardKind, CardRareLevel, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int resonanceTotal = GetTotalResonanceCount(base.Owner.Creature);
        int markAmount = resonanceTotal + (int)DynamicVars["Mark"].BaseValue;

        if (resonanceTotal > 0)
        {
            await DamageCmd.Attack(resonanceTotal)
                .FromCard(this, cardPlay)
                .Targeting(cardPlay.Target!)
                .Unpowered()
                .Execute(choiceContext);
        }

        await FiveSpiritMarkPower.ApplyMark(
            choiceContext,
            base.Owner.Creature,
            markAmount,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Mark"].UpgradeValueBy(1);
    }

    private static int GetTotalResonanceCount(Creature creature)
    {
        return GetPowerAmount<IronTigerResonanceCountPower>(creature)
            + GetPowerAmount<WoodApeResonanceCountPower>(creature)
            + GetPowerAmount<IceDeerResonanceCountPower>(creature)
            + GetPowerAmount<FireCraneResonanceCountPower>(creature)
            + GetPowerAmount<StoneBearResonanceCountPower>(creature);
    }

    private static int GetPowerAmount<TPower>(Creature creature)
        where TPower : PowerModel
    {
        TPower? power = creature.GetPower<TPower>();
        return power == null ? 0 : (int)power.Amount;
    }

    private static int GetTotalResonanceCountFromCard(CardModel card)
    {
        if (card.Owner == null)
        {
            return 0;
        }

        return GetTotalResonanceCount(card.Owner.Creature);
    }

    /// <summary>
    /// 智能显示：当前五种形态共鸣次数之和。
    /// </summary>
    private sealed class ResonanceTotalVar : DynamicVar
    {
        public ResonanceTotalVar()
            : base("Resonance", 0)
        {
        }

        public override void UpdateCardPreview(
            CardModel card,
            CardPreviewMode previewMode,
            Creature? target,
            bool runGlobalHooks
        )
        {
            decimal total = GetTotalResonanceCountFromCard(card);

            EnchantedValue = total;
            PreviewValue = total;
        }
    }

    /// <summary>
    /// 智能显示：刻印数量。
    /// 未升级 = 共鸣次数之和。
    /// 升级后 = 共鸣次数之和 + 1。
    /// </summary>
    private sealed class ComboMarkVar : DynamicVar
    {
        public ComboMarkVar()
            : base("Mark", 0)
        {
        }

        public override void UpdateCardPreview(
            CardModel card,
            CardPreviewMode previewMode,
            Creature? target,
            bool runGlobalHooks
        )
        {
            decimal total = GetTotalResonanceCountFromCard(card);

            EnchantedValue = total;
            PreviewValue = total + BaseValue;
        }
    }
}