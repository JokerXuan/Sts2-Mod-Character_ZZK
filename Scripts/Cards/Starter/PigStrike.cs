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

namespace Character_ZZK.Scripts.Cards.Starter;

/// <summary>
/// 猪猪凯的起始攻击牌。
/// M1 阶段先用它验证：卡牌注册、起始卡组注册、伤害逻辑、本地化、卡图路径。
/// </summary>
[RegisterCard(typeof(ZzkCardPool))]
[RegisterCharacterStarterCard(typeof(PigKaiCharacter), 5)]
public class PigStrike : ModCardTemplate
{
    // 这几个常量只在构造函数里使用。
    // 名字不要叫 EnergyCost / Type / Rarity / TargetType，
    // 否则会和父类 CardModel 的成员重名。
    private const int BaseEnergyCost = 1;
    private const CardType CardKind = CardType.Attack;
    private const CardRarity CardRareLevel = CardRarity.Common;
    private const TargetType CardTarget = TargetType.AnyEnemy;
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        // M1 阶段先用 Godot 默认 icon.svg 占位。
        // 后面有卡图后，改成：
        // res://Character_ZZK/images/cards/PigStrike.png
        PortraitPath: "res://icon.svg"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6, ValueProp.Move)
    ];

    public PigStrike()
        : base(BaseEnergyCost, CardKind, CardRareLevel, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}