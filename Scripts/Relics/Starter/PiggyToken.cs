using MegaCrit.Sts2.Core.Entities.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Character_ZZK.Scripts.Characters;

namespace Character_ZZK.Scripts.Relics.Starter;

/// <summary>
/// 猪猪凯的起始遗物。
/// M1 阶段先做成无效果遗物，只用于验证起始遗物注册链路。
/// </summary>
[RegisterRelic(typeof(ZzkRelicPool))]
[RegisterCharacterStarterRelic(typeof(PigKaiCharacter))]
public class PiggyToken : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override RelicAssetProfile AssetProfile => new(
        // M1 阶段先用 Godot 默认 icon.svg 占位。
        // 后面有遗物图标后，改成：
        // res://Character_ZZK/images/relics/PiggyToken.png
        IconPath: "res://icon.svg",
        IconOutlinePath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );
}