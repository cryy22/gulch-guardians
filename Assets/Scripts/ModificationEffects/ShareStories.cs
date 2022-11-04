using System.Collections;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(
        fileName = "ShareStories",
        menuName = "Scriptable Objects/Modification Effects/Share Stories"
    )]
    public class ShareStories : ModificationEffect
    {
        private const string _name = "Share Stories";
        public override string Name => _name;
        public override TargetType Target => TargetType.PlayerTeam;

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);

            yield return CoroutineHelper.RunConcurrently(
                behaviours: context.PlayerTeam!.Units,
                u => u.Upgrade(attack: 1, health: 0)
            );
        }
    }
}
