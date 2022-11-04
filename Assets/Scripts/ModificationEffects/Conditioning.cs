using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GulchGuardians
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

        public override IEnumerator Apply(Unit unit = null, Team team = null)
        {
            yield return base.Apply(unit: unit, team: team);

            IEnumerable<Coroutine> coroutines = team!.Units.Select(
                teamUnit => teamUnit.StartCoroutine(teamUnit.Upgrade(attack: 0, health: 1))
            );
            foreach (Coroutine coroutine in coroutines) yield return coroutine;
        }
    }
}
