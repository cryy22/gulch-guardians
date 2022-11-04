using UnityEngine;

namespace InfiniteSAPPrototype
{
    [CreateAssetMenu(
        fileName = "HotSprings",
        menuName = "Scriptable Objects/Modification Effects/Hot Springs"
    )]
    public class HotSprings : ModificationEffect
    {
        private const string _name = "Hot Springs";

        public override string Name => _name;
        public override TargetType Target => TargetType.Unit;

        public override void Apply(Unit unit = null, UnitTeam team = null)
        {
            base.Apply(unit: unit, team: team);

            unit!.FullHeal();
        }
    }
}