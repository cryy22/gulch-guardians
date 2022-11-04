using UnityEngine;

namespace InfiniteSAPPrototype
{
    [CreateAssetMenu(
        fileName = "Conditioning",
        menuName = "Scriptable Objects/Modification Effects/Conditioning"
    )]
    public class Conditioning : ModificationEffect
    {
        private const string _name = "Conditioning";

        public override string Name => _name;
        public override TargetType Target => TargetType.Team;

        public override void Apply(Unit unit = null, UnitTeam team = null)
        {
            base.Apply(unit: unit, team: team);
            foreach (Unit teamUnit in team!.Units) teamUnit.Upgrade(attack: 0, health: 1);
        }
    }
}
