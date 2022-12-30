using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Presentation;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Squads
{
    public class SquadArrangement : MonoBehaviour, IArrangement<Unit>, IArrangementElement
    {
        private Arrangement _arrangement;

        private void Awake() { _arrangement = GetComponent<Arrangement>(); }

        public void UpdateProperties() { _arrangement.UpdateProperties(); }

        // IArrangement<Squad> implementation
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

        public void SetElements(IEnumerable<Unit> elements) { _arrangement.SetElements(elements.Select(u => u.View)); }

        public void Rearrange() { _arrangement.Rearrange(); }

        public IEnumerator AnimateRearrange(float duration = 0.25f) { return _arrangement.AnimateRearrange(duration); }

        // IArrangementElement implementation
        public Transform Transform => transform;
        public Vector2 Pivot => _arrangement.Pivot;
        public Vector2 SizeMultiplier => _arrangement.SizeMultiplier;
    }
}
