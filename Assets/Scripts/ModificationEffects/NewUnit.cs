using System.Collections;
using System.Linq;
using GulchGuardians.Teams;
using GulchGuardians.Units;
using Unity.VisualScripting;
using UnityEngine;
using Unit = GulchGuardians.Units.Unit;

namespace GulchGuardians.ModificationEffects
{
    [CreateAssetMenu(fileName = "NewUnit", menuName = "Modification Effects/New Unit")]
    public class NewUnit : ModificationEffect
    {
        [SerializeField] private UnitSet UnitSet;

        [DoNotSerialize] private Unit _unit;

        public override TargetType Target => TargetType.PlayerTeam;

        public override bool CanBeAppliedTo(Context context)
        {
            Team playerTeam = context.PlayerTeam;
            return playerTeam != null && playerTeam.Units.Count < playerTeam.MaxUnits;
        }

        public override void Prepare()
        {
            base.Prepare();
            _unit = UnitSet.GenerateUnits().First();
            _unit.TooltipEnabled = false;
            _unit.SetNametagActive(false);
            _unit.gameObject.SetActive(false);
        }

        public override IEnumerator Apply(Context context)
        {
            yield return base.Apply(context);
            yield return context.PlayerTeam!.AddUnit(_unit);
            _unit.TooltipEnabled = true;
            _unit.SetNametagActive(true);
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
