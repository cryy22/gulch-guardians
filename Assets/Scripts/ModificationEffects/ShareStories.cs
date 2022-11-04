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
        public override TargetType Target => TargetType.Team;

        public override void Apply(Unit unit = null, Team team = null)
        {
            base.Apply(unit: unit, team: team);
            foreach (Unit teamUnit in team!.Units) teamUnit.Upgrade(attack: 1, health: 0);
        }
    }
}
