using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(
        fileName = "New UnitStatsEffect",
        menuName = "Scriptable Objects/Modification Effects/Unit Stats"
    )]
    public class UnitStatsEffect : ModificationEffect
    {
        [SerializeField] private int AttackChange;
        [SerializeField] private int HealthChange;

        [SerializeField] private TargetType TargetType;

        public override TargetType Target => TargetType;

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);

            IEnumerable<Unit> targets = TargetType switch
            {
                TargetType.Unit              => new List<Unit> { context.Unit! },
                TargetType.PlayerTeam        => context.PlayerTeam.Units!,
                TargetType.UnitAndPlayerTeam => context.PlayerTeam.Units!,
                TargetType.EnemyTeam         => context.EnemyTeam.Units!.Take(context.EnemyTeam.UnitsInCombatCycle),
                _                            => new List<Unit>(),
            };

            yield return CoroutineHelper.RunConcurrently(
                behaviours: targets,
                u => u.Upgrade(attack: AttackChange, health: HealthChange)
            );
        }
    }
}
