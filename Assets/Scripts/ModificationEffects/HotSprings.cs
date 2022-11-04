using System.Collections;
using UnityEngine;

namespace GulchGuardians
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

        public override IEnumerator Apply(Unit unit = null, Team team = null)
        {
            yield return base.Apply(unit: unit, team: team);

            unit!.FullHeal();
        }
    }
}
