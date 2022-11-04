using System.Collections;
using UnityEngine;

namespace GulchGuardians
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

        public override IEnumerator Apply(Unit unit = null, Team team = null)
        {
            yield return base.Apply(unit: unit, team: team);
            yield return unit!.Upgrade(attack: 2, health: 0);
        }
    }
}
