using System.Collections;
using System.Linq;
using Abilities;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(
        fileName = "New AddAbilityEffect",
        menuName = "Scriptable Objects/Modification Effects/Add Ability"
    )]
    public class AddAbilityEffect : ModificationEffect
    {
        [SerializeField] private AbilityType Ability;

        public override TargetType Target => TargetType.Unit;

        public override bool CanBeAppliedTo(Context context)
        {
            Team playerTeam = context.PlayerTeam;
            if (playerTeam == null) return false;

            if (Ability.IsBadForSoloTeam && playerTeam.Units.Count <= 1) return false;
            return playerTeam.Units.Any(u => !u.HasAbility(Ability));
        }

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);
            yield return context.Unit!.AddAbility(Ability);
        }
    }
}
