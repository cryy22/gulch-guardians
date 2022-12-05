using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using GulchGuardians.Teams;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.ModificationEffects
{
    [CreateAssetMenu(fileName = "New UnitStatsEffect", menuName = "Modification Effects/Unit Stats")]
    public class UnitStatsEffect : ModificationEffect
    {
        [SerializeField] private int AttackChange;
        [SerializeField] private int HealthChange;

        [SerializeField] private TargetType IntendedTarget;

        public override TargetType Target => IntendedTarget;

        public override bool CanBeAppliedTo(Context context)
        {
            switch (IntendedTarget)
            {
                case TargetType.PlayerTeam:
                case TargetType.UnitAndPlayerTeam:
                    Team playerTeam = context.PlayerTeam;
                    return playerTeam != null && playerTeam.Units.Count() > 1;
                case TargetType.Unit:
                case TargetType.EnemyTeam:
                default:
                    return true;
            }
        }

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);

            IEnumerable<Unit> targets = IntendedTarget switch
            {
                TargetType.Unit              => new List<Unit> { context.Unit! },
                TargetType.PlayerTeam        => context.PlayerTeam.Units!,
                TargetType.UnitAndPlayerTeam => context.PlayerTeam.Units!,
                TargetType.EnemyTeam         => context.EnemyTeam.FrontSquad.Units!,
                _                            => new List<Unit>(),
            };

            yield return CoroutineWaiter.RunConcurrently(
                behaviours: targets,
                u => u.Upgrade(attack: AttackChange, health: HealthChange)
            );
        }
    }
}
