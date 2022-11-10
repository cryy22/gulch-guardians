using System.Collections;
using System.Linq;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(
        fileName = "HotSprings",
        menuName = "Scriptable Objects/Modification Effects/Hot Springs"
    )]
    public class HotSprings : ModificationEffect
    {
        public override TargetType Target => TargetType.Unit;

        public override bool CanBeAppliedTo(Context context)
        {
            Team playerTeam = context.PlayerTeam;
            return playerTeam != null && playerTeam.Units.Any(u => u.Health < u.MaxHealth);
        }

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);
            yield return context.Unit!.FullHeal();
        }
    }
}
