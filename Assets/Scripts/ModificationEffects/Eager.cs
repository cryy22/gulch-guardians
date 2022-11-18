using System.Collections;
using GulchGuardians.Helpers;
using GulchGuardians.Teams;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.ModificationEffects
{
    [CreateAssetMenu(fileName = "Eager", menuName = "Modification Effects/Eager")]
    public class Eager : ModificationEffect
    {
        public override TargetType Target => TargetType.UnitAndPlayerTeam;

        public override bool CanBeAppliedTo(Context context)
        {
            Team playerTeam = context.PlayerTeam;
            return playerTeam != null && playerTeam.Units.Count > 1;
        }

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);

            Team playerTeam = context.PlayerTeam!;
            Unit unit = context.Unit!;

            yield return CoroutineWaiter.RunConcurrently(
                playerTeam.StartCoroutine(playerTeam.SetUnitIndex(unit: unit, index: 0)),
                unit.StartCoroutine(unit.Upgrade(attack: 1, health: 0))
            );
        }
    }
}
