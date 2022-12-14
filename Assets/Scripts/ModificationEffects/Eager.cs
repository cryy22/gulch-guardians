using System.Collections;
using System.Linq;
using Crysc.Helpers;
using GulchGuardians.Squads;
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
            return playerTeam != null && playerTeam.Units.Count() > 1;
        }

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);

            Squad playerSquad = context.PlayerTeam!.FrontSquad;
            Unit unit = context.Unit!;

            yield return CoroutineWaiter.RunConcurrently(
                playerSquad.StartCoroutine(playerSquad.SetUnitIndex(unit: unit, index: 0)),
                unit.StartCoroutine(unit.Upgrade(attack: 1, health: 0))
            );
        }
    }
}
