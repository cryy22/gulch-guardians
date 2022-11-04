using UnityEngine;

namespace InfiniteSAPPrototype
{
    [CreateAssetMenu(
        fileName = "HoneBlade",
        menuName = "Scriptable Objects/Modification Effects/Hone Blade"
    )]
    public class HoneBlade : ModificationEffect
    {
        private const string _name = "Hone Blade";

        public override string Name => _name;
        public override TargetType Target => TargetType.Unit;

        public override void Apply(Unit unit = null, UnitTeam team = null)
        {
            base.Apply(unit: unit, team: team);

            unit!.Upgrade(attack: 2, health: 0);
        }
    }
}