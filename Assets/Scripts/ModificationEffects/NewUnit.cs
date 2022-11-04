using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(fileName = "NewUnit", menuName = "Scriptable Objects/Modification Effects/New Unit")]
    public class NewUnit : ModificationEffect
    {
        private const string _name = "New Unit";

        [SerializeField] private UnitSet UnitSet;

        [DoNotSerialize] private Unit _unit;

        public override string Name => _name;
        public override TargetType Target => TargetType.Team;

        public override bool CanBeAppliedTo(Team team = null)
        {
            return team != null && team.Units.Count < team.MaxUnits;
        }

        public override void Prepare()
        {
            base.Prepare();
            _unit = UnitSet.GenerateUnits().First();
            _unit.gameObject.SetActive(false);
        }

        public override IEnumerator Apply(Unit unit = null, Team team = null)
        {
            yield return base.Apply(unit: unit, team: team);

            team!.AddUnit(_unit);
            _unit = null;
        }

        public override void CleanUp()
        {
            base.CleanUp();
            if (_unit != null) Destroy(_unit.gameObject);
        }

        public override GameObject GetPreviewGameObject()
        {
            _unit.gameObject.SetActive(true);
            return _unit.gameObject;
        }
    }
}
