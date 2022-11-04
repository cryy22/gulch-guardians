using UnityEngine;

namespace InfiniteSAPPrototype
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

        public override void Apply(Unit unit = null, UnitTeam team = null)
        {
            base.Apply(unit: unit, team: team);

            int targetIndex = team!.Units.IndexOf(unit) - 1;
            if (targetIndex < 0) targetIndex = team.Units.Count - 1;

            team.SetUnitIndex(unit: unit, index: targetIndex);
        }
    }
}
