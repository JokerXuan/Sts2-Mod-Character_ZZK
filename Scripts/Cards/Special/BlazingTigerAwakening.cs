using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Character_ZZK.Scripts.Characters;
using Character_ZZK.Scripts.Powers;

namespace Character_ZZK.Scripts.Cards.Special;

/// <summary>
/// 五灵形态卡之一：炽虎觉醒。
/// 由五灵锁生成，打出后进入“炽虎形态”。
/// </summary>
[RegisterCard(typeof(ZzkCardPool))]
public class BlazingTigerAwakening : ModCardTemplate
{
    private const int BaseEnergyCost = 0;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRareLevel = CardRarity.Token;
    private const TargetType CardTarget = TargetType.Self;

    // true：显示在局外图鉴。
    // 奖励池排除主要靠 Token 稀有度 + 生成限制 + 后续必要时做奖励过滤。
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://icon.svg"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    // 形态卡应该打出后消耗，并且由五灵锁生成到手牌时最好可保留。
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust,
        CardKeyword.Retain
    ];

    // 防止被随机战斗生成类效果生成。
    public override bool CanBeGeneratedInCombat => false;

    // 防止被部分随机修饰/生成机制选中。
    public override bool CanBeGeneratedByModifiers => false;

    public BlazingTigerAwakening()
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

        await PowerCmd.Apply<BlazingTigerFormPower>(
            choiceContext,
            new[] { base.Owner.Creature },
            1m,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 五灵形态卡暂时不设计升级。
    }
}