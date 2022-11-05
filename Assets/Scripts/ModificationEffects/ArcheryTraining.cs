using System.Collections;
using System.Linq;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(
        fileName = "ArcheryTraining",
        menuName = "Scriptable Objects/Modification Effects/Archery Training"
    )]
    public class ArcheryTraining : ModificationEffect
    {
        private const string _name = "Archery Trng";
        public override string Name => _name;

        public override TargetType Target => TargetType.Unit;

        public override bool CanBeAppliedTo(Context context)
        {
            Team playerTeam = context.PlayerTeam;
            return playerTeam != null && playerTeam.Units.Any(u => !u.IsArcher);
        }

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);
            yield return context.Unit!.AddArcher();
        }
    }
}
