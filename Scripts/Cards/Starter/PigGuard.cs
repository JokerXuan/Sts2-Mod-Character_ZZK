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
/// 猪猪凯的起始防御牌。
/// M1 阶段用它验证：技能牌注册、起始卡组注册、格挡逻辑、本地化、卡图路径。
/// </summary>
[RegisterCard(typeof(ZzkCardPool))]
[RegisterCharacterStarterCard(typeof(PigKaiCharacter), 5)]
public class PigGuard : ModCardTemplate
{
    // 这几个常量只在构造函数里使用。
    // 不要命名成 EnergyCost / Type / Rarity / TargetType，
    // 否则容易和父类 CardModel 的成员重名。
    private const int BaseEnergyCost = 1;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRareLevel = CardRarity.Common;
    private const TargetType CardTarget = TargetType.Self;
    private const bool ShowInCardLibrary = true;

    // 告诉游戏：这是一张会获得格挡的卡。
    public override bool GainsBlock => true;

    public override CardAssetProfile AssetProfile => new(
        // M1 阶段先用 Godot 默认 icon.svg 占位。
        // 后面有卡图后，改成：
        // res://Character_ZZK/images/cards/PigGuard.png
        PortraitPath: "res://icon.svg"
    );

    // 这张牌的基础数值：获得 5 点格挡。
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5, ValueProp.Move)
    ];

    public PigGuard()
        : base(BaseEnergyCost, CardKind, CardRareLevel, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}