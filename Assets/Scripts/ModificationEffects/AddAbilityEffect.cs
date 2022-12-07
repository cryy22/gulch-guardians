using System.Collections;
using System.Linq;
using GulchGuardians.Abilities;
using GulchGuardians.Teams;
using UnityEngine;

namespace GulchGuardians.ModificationEffects
{
    [CreateAssetMenu(fileName = "New AddAbilityEffect", menuName = "Modification Effects/Add Ability")]
    public class AddAbilityEffect : ModificationEffect
    {
        [SerializeField] private AbilityType Ability;

        public override TargetType Target => TargetType.Unit;

        public override bool CanBeAppliedTo(Context context)
        {
            Team playerTeam = context.PlayerTeam;
            if (playerTeam == null) return false;

            if (Ability.IsBadForSoloTeam && playerTeam.Units.Count() <= 1) return false;
            if (playerTeam.Units.Count(u => u.HasAbility(Ability)) >= Ability.MaxPerTeam) return false;
            return playerTeam.Units.Any(u => !u.HasAbility(Ability));
        }

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);
            yield return context.Unit!.AddAbility(Ability);
        }
    }
}
