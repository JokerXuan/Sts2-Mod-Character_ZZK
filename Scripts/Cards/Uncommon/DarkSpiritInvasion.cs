using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Character_ZZK.Scripts.Characters;
using Character_ZZK.Scripts.Powers.Buffs;

namespace Character_ZZK.Scripts.Cards.Uncommon;

/// <summary>
/// 暗灵入侵。
/// 对所有敌人造成伤害。如果拥有至少10层灵源印记，施加易伤和虚弱。
/// </summary>
[RegisterCard(typeof(ZzkCardPool))]
public class DarkSpiritInvasion : ModCardTemplate
{
    private const int BaseEnergyCost = 2;
    private const CardType CardKind = CardType.Attack;
    private const CardRarity CardRareLevel = CardRarity.Uncommon;
    private const TargetType CardTarget = TargetType.AllEnemies;
    private const bool ShowInCardLibrary = true;

    private const int RequiredMarkAmount = 10;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://icon.svg"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move),
        ModCardVars.Int("Debuff", 2)
    ];

    public DarkSpiritInvasion()
        : base(BaseEnergyCost, CardKind, CardRareLevel, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ICombatState? combatState = base.Owner.Creature.CombatState;

        if (combatState == null || !combatState.IsLiveCombat())
        {
            return;
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);

        int markAmount = FiveSpiritMarkPower.GetMarkAmount(base.Owner.Creature);

        if (markAmount < RequiredMarkAmount)
        {
            return;
        }

        List<Creature> enemies = combatState.HittableEnemies
            .Where(enemy => enemy.IsAlive)
            .ToList();

        if (enemies.Count == 0)
        {
            return;
        }

        await PowerCmd.Apply<VulnerablePower>(
            choiceContext,
            enemies,
            DynamicVars["Debuff"].BaseValue,
            base.Owner.Creature,
            this
        );

        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            enemies,
            DynamicVars["Debuff"].BaseValue,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}