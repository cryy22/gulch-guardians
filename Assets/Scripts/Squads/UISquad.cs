using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.UI;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Squads
{
    [RequireComponent(typeof(UIArrangement))]
    public class UISquad : MonoBehaviour, IArrangementElement
    {
        private UIArrangement _arrangement;

        private void Awake() { _arrangement = GetComponent<UIArrangement>(); }

        public void Setup(Squad squad) { _arrangement.UpdateElements(squad.Units.Select(u => u.UI)); }

        public void SetArrangementOrderInversion(bool isInverted) { _arrangement.SetOrderInversion(isInverted); }

        public IEnumerator AnimateUpdateElements(IEnumerable<Unit> units)
        {
            yield return _arrangement.AnimateUpdateElements(units.Select(u => u.UI));
        }

        public void UpdateMaxSize(Vector2 maxSize) { _arrangement.UpdateMaxSize(maxSize); }

        public IEnumerator AnimateUpdateMaxSize(Vector2 maxSize)
        {
            yield return _arrangement.AnimateUpdateMaxSize(maxSize);
        }

        public IEnumerator AnimateUpdateUnitIndex(IEnumerable<Unit> units, Unit unit, bool withHurtAnimation = false)
        {
            if (withHurtAnimation) unit.SetHurtAnimation();
            yield return AnimateUpdateElements(units);
            if (withHurtAnimation) unit.SetIdleAnimation();
        }

        // IArrangementElement
        public Transform Transform => _arrangement.Transform;
        public Vector2 SpacingMultiplier => _arrangement.SpacingMultiplier;
        public Vector2 Pivot => _arrangement.Pivot;
    }
}
