using System.Collections;
using System.Collections.Generic;
using Crysc.UI;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Squads
{
    [RequireComponent(typeof(UIArrangement))]
    public class UISquad : MonoBehaviour
    {
        private UIArrangement _arrangement;

        private void Awake() { _arrangement = GetComponent<UIArrangement>(); }

        public void Setup(Squad squad) { _arrangement.UpdateElements(squad.Units); }

        public void InvertArrangementOrder() { _arrangement.InvertOrder(); }

        public IEnumerator AnimateUpdateElements(IEnumerable<Unit> units)
        {
            yield return _arrangement.AnimateUpdateElements(units);
        }

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
    }
}
