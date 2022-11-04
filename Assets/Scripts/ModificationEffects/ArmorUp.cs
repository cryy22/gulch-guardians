using UnityEngine;

namespace InfiniteSAPPrototype
{
    [CreateAssetMenu(
        fileName = "ArmorUp",
        menuName = "Scriptable Objects/Modification Effects/Armor Up"
    )]
    public class ArmorUp : ModificationEffect
    {
        private const string _name = "Armor Up";

        public override string Name => _name;
        public override TargetType Target => TargetType.Unit;

        public override void Apply(Unit unit = null, UnitTeam team = null)
        {
            base.Apply(unit: unit, team: team);

            unit!.Upgrade(attack: 0, health: 2);
        }
    }
}