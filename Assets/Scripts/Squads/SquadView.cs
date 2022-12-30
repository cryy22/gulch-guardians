using System.Collections;
using Crysc.Presentation;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Squads
{
    [RequireComponent(typeof(Arrangement))]
    public class SquadView : MonoBehaviour, IArrangementElement
    {
        private Squad _squad;
        public SquadArrangement Arrangement { get; private set; }

        private void Awake()
        {
            _squad = GetComponent<Squad>();
            Arrangement = GetComponent<SquadArrangement>();
        }

        public void UpdateArrangement()
        {
            Arrangement.SetElements(_squad.Units);
            Arrangement.Rearrange();
        }

        public IEnumerator AnimateUpdateArrangement(float duration = 0.25f)
        {
            Arrangement.SetElements(_squad.Units);
            return Arrangement.AnimateRearrange(duration);
        }

        public IEnumerator AnimateUpdateUnitIndex(Unit unit, bool withHurtAnimation = false)
        {
            if (withHurtAnimation) unit.SetHurtAnimation();
            yield return AnimateUpdateArrangement();
            if (withHurtAnimation) unit.SetIdleAnimation();
        }

        public void ShowUnitUIs(bool show)
        {
            foreach (Unit unit in _squad.Units) unit.View.SetShowDetails(show);
        }

        // IArrangementElement
        public Transform Transform => Arrangement.Transform;
        public Vector2 SizeMultiplier => Arrangement.SizeMultiplier;
        public Vector2 Pivot => Arrangement.Pivot;
    }
}
