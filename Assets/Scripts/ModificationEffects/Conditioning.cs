using System.Collections;
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
        public override TargetType Target => TargetType.PlayerTeam;

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);

            yield return CoroutineHelper.RunConcurrently(
                behaviours: context.PlayerTeam!.Units,
                u => u.Upgrade(attack: 0, health: 1)
            );
        }
    }
}
