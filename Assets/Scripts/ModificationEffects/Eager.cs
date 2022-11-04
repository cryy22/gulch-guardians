using System.Collections;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(
        fileName = "Eager",
        menuName = "Scriptable Objects/Modification Effects/Eager"
    )]
    public class Eager : ModificationEffect
    {
        private const string _name = "Eager";

        public override string Name => _name;
        public override TargetType Target => TargetType.Both;

        public override bool CanBeAppliedTo(Team team = null) { return team != null && team.Units.Count > 1; }

        public override IEnumerator Apply(Unit unit = null, Team team = null)
        {
            yield return base.Apply(unit: unit, team: team);

            int targetIndex = team!.Units.IndexOf(unit) - 1;
            if (targetIndex < 0) targetIndex = team.Units.Count - 1;

            yield return team.SetUnitIndex(unit: unit, index: targetIndex);
        }
    }
}
