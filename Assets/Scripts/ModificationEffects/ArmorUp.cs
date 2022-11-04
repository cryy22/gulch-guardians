using System.Collections;
using UnityEngine;

namespace GulchGuardians
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

        public override IEnumerator Apply(Unit unit = null, Team team = null)
        {
            yield return base.Apply(unit: unit, team: team);
            yield return unit!.Upgrade(attack: 0, health: 2);
        }
    }
}
