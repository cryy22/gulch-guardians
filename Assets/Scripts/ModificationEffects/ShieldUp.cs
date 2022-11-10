using System.Collections;
using System.Linq;
using Abilities;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(
        fileName = "ShieldUp",
        menuName = "Scriptable Objects/Modification Effects/Shield Up"
    )]
    public class ShieldUp : ModificationEffect
    {
        private const string _name = "Shield Up";
        public override string Name => _name;

        public override TargetType Target => TargetType.Unit;

        public override bool CanBeAppliedTo(Context context)
        {
            Team playerTeam = context.PlayerTeam;
            return playerTeam != null && playerTeam.Units.Any(u => !u.HasAbility(Ability.Sturdy));
        }

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);
            yield return context.Unit!.AddAbility(Ability.Sturdy);
        }
    }
}
