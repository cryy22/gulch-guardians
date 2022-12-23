using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.UI;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Squads
{
    [RequireComponent(typeof(UIArrangement))]
    public class UISquad : MonoBehaviour, IArrangement, IArrangementElement
    {
        private UIArrangement _arrangement;

        private void Awake() { _arrangement = GetComponent<UIArrangement>(); }

        public void Setup(Squad squad) { _arrangement.UpdateElements(squad.Units.Select(u => u.UI)); }

        public IEnumerator AnimateUpdateUnitIndex(IEnumerable<Unit> units, Unit unit, bool withHurtAnimation = false)
        {
            if (withHurtAnimation) unit.SetHurtAnimation();
            yield return _arrangement.AnimateUpdateElements(units.Select(u => u.UI));
            if (withHurtAnimation) unit.SetIdleAnimation();
        }

        // IArrangement
        public bool IsCentered
        {
            get => _arrangement.IsCentered;
            set => _arrangement.IsCentered = value;
        }

        public bool IsInverted
        {
            get => _arrangement.IsInverted;
            set => _arrangement.IsInverted = value;
        }

        public Vector2 MaxSize
        {
            get => _arrangement.MaxSize;
            set => _arrangement.MaxSize = value;
        }

        public void SetElements(IEnumerable<IArrangementElement> elements) { _arrangement.SetElements(elements); }
        public void Rearrange() { _arrangement.Rearrange(); }
        public IEnumerator AnimateRearrange(float duration = 0.25f) { return _arrangement.AnimateRearrange(duration); }

        // IArrangementElement
        public Transform Transform => _arrangement.Transform;
        public Vector2 SpacingMultiplier => _arrangement.SpacingMultiplier;
        public Vector2 Pivot => _arrangement.Pivot;
    }
}
