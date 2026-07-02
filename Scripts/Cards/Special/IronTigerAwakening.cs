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
using Character_ZZK.Scripts.Powers;
using Character_ZZK.Scripts.Mechanics;

namespace Character_ZZK.Scripts.Cards.Special;

/// <summary>
/// 五灵形态卡之一：铁虎觉醒。
/// 由五灵锁生成，打出后进入“铁虎形态”。
/// </summary>
[RegisterCard(typeof(ZzkCardPool))]
public class IronTigerAwakening : ModCardTemplate
{
    private const int BaseEnergyCost = 2;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRareLevel = CardRarity.Token;
    private const TargetType CardTarget = TargetType.Self;

    // true：显示在局外图鉴。
    // Token + 禁止随机生成：表达“这是五灵锁生成的机制牌，不是普通奖励牌”。
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://icon.svg"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust,
        CardKeyword.Retain
    ];

    public override bool CanBeGeneratedInCombat => false;

    public override bool CanBeGeneratedByModifiers => false;

    public IronTigerAwakening()
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

        await FiveSpiritFormUtils.RemoveOtherForms<IronTigerFormPower>(
            base.Owner.Creature
        );

        await PowerCmd.Apply<IronTigerFormPower>(
            choiceContext,
            base.Owner.Creature,
            1m,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 五灵形态卡暂时不设计升级。
    }

    /// <summary>
    /// 将一张“铁虎觉醒”生成到指定玩家手牌。
    /// 这个方法供“五灵锁”等遗物调用。
    /// </summary>
    public static async Task CreateInHand(Player owner, ICombatState combatState)
    {
        if (!combatState.IsLiveCombat())
        {
            return;
        }

        IronTigerAwakening card = combatState.CreateCard<IronTigerAwakening>(owner);

        await CardPileCmd.AddGeneratedCardToCombat(
            card,
            PileType.Hand,
            owner
        );
    }
}