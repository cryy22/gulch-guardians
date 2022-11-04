using System.Collections;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(
        fileName = "Eager",
        menuName = "Scriptable Objects/Modification Effects/Eager"
    )]
    public class Eager : ModificationEffect
    {
        private const string _name = "Eager";

        public override string Name => _name;
        public override TargetType Target => TargetType.UnitAndPlayerTeam;

        public override bool CanBeAppliedTo(Context context)
        {
            Team playerTeam = context.PlayerTeam;
            return playerTeam != null && playerTeam.Units.Count > 1;
        }

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);

            Team playerTeam = context.PlayerTeam!;
            Unit unit = context.Unit!;

            int targetIndex = playerTeam!.Units.IndexOf(unit) - 1;
            if (targetIndex < 0) targetIndex = playerTeam.Units.Count - 1;

            yield return playerTeam.SetUnitIndex(unit: unit, index: targetIndex);
        }
    }
}
