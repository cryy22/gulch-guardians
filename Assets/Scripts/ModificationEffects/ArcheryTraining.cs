using System.Collections;
using System.Linq;
using Abilities;
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
        [SerializeField] private AbilityType ArcherType;

        public override string Name => _name;

        public override TargetType Target => TargetType.Unit;

        public override bool CanBeAppliedTo(Context context)
        {
            Team playerTeam = context.PlayerTeam;
            return playerTeam != null && playerTeam.Units.Any(u => !u.HasAbility(ArcherType));
        }

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);
            yield return context.Unit!.AddAbility(ArcherType);
        }
    }
}
