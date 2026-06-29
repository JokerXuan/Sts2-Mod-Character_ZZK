using MegaCrit.Sts2.Core.Entities.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Character_ZZK.Scripts.Characters;

namespace Character_ZZK.Scripts.Relics.Starter;

/// <summary>
/// 猪猪凯的初始遗物：五灵锁。
/// </summary>
[RegisterRelic(typeof(ZzkRelicPool))]
[RegisterCharacterStarterRelic(typeof(PigKaiCharacter))]
public class FiveSpiritLock : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override RelicAssetProfile AssetProfile => new(
        // M2 阶段先用 Godot 默认 icon.svg 占位。
        // 后面替换为：
        // res://Character_ZZK/images/relics/FiveSpiritLock.png
        IconPath: "res://icon.svg",
        IconOutlinePath: "res://icon.svg",
        BigIconPath: "res://icon.svg"
    );
}