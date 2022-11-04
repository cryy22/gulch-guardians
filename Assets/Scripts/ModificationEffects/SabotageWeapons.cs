using System.Collections;
using System.Linq;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(
        fileName = "SabotageWeapons",
        menuName = "Scriptable Objects/Modification Effects/Sabotage Weapons"
    )]
    public class SabotageWeapons : ModificationEffect
    {
        private const string _name = "Sabotage Weps";

        public override string Name => _name;
        public override TargetType Target => TargetType.EnemyTeam;

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);

            Team enemyTeam = context.EnemyTeam!;
            yield return CoroutineHelper.RunConcurrently(
                behaviours: enemyTeam.Units.Take(enemyTeam.UnitsInCombatCycle),
                u => u.Upgrade(attack: -1, health: 0)
            );
        }
    }
}
