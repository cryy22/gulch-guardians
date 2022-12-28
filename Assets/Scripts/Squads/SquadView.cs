using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.UI;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Squads
{
    [RequireComponent(typeof(UIArrangement))]
    public class SquadView : MonoBehaviour, IArrangement<UnitView>, IArrangementElement
    {
        private readonly List<UnitView> _units = new();
        private UIArrangement _arrangement;

        private void Awake() { _arrangement = GetComponent<UIArrangement>(); }

        public void Setup(Squad squad)
        {
            SetElements(squad.Units.Select(u => u.View));
            Rearrange();
        }

        public IEnumerator AnimateUpdateUnitIndex(IEnumerable<Unit> units, Unit unit, bool withHurtAnimation = false)
        {
            if (withHurtAnimation) unit.SetHurtAnimation();

            _arrangement.SetElements(units.Select(u => u.View));
            yield return _arrangement.AnimateRearrange();

            if (withHurtAnimation) unit.SetIdleAnimation();
        }

        public void ShowUnitUIs(bool show)
        {
            foreach (UnitView unit in _units) unit.ShowUI(show);
        }

        public void UpdateProperties() { _arrangement.UpdateProperties(); }

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

        public Vector2 PreferredSpacingRatio
        {
            get => _arrangement.PreferredSpacingRatio;
            set => _arrangement.PreferredSpacingRatio = value;
        }

        public void SetElements(IEnumerable<UnitView> elements)
        {
            _units.Clear();
            _units.AddRange(elements);

            _arrangement.SetElements(elements);
        }

        public void Rearrange() { _arrangement.Rearrange(); }
        public IEnumerator AnimateRearrange(float duration = 0.25f) { return _arrangement.AnimateRearrange(duration); }

        // IArrangementElement
        public Transform Transform => _arrangement.Transform;
        public Vector2 SizeMultiplier => _arrangement.SizeMultiplier;
        public Vector2 Pivot => _arrangement.Pivot;
    }
}
