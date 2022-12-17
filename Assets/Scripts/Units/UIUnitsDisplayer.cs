using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using GulchGuardians.Squads;
using UnityEngine;

namespace GulchGuardians.Units
{
    public class UIUnitsDisplayer : MonoBehaviour
    {
        public float UnitSpacingX = 1.8825f;
        public float UnitStaggerY = 0.25f;
        public bool IsInverted;

        [SerializeField] private bool DemarcatesRounds;

        [SerializeField] private Transform UnitsParent;
        [SerializeField] private GameObject RoundDemarcation;

        private readonly Dictionary<Unit, Vector3> _unitsPositions = new();
        private Squad _frontSquad;

        private void Awake() { RoundDemarcation.SetActive(DemarcatesRounds); }

        public IEnumerator AnimateUpdateDisplay(IEnumerable<Unit> units)
        {
            bool hasChanged = UpdateUnitsAndPositions(units);
            if (!hasChanged) yield break;

            yield return CoroutineWaiter.RunConcurrently(
                StartCoroutine(
                    CoroutineWaiter.RunConcurrently(
                        behaviours: _unitsPositions.Keys,
                        unit => unit.MoveToPosition(position: _unitsPositions[unit], duration: 0.25f)
                    )
                ),
                StartCoroutine(AnimateMoveDemarcation(duration: 0.25f))
            );
        }

        public void UpdateDisplay(IEnumerable<Unit> units)
        {
            bool hasChanged = UpdateUnitsAndPositions(units);
            if (!hasChanged) return;

            foreach (Unit unit in _unitsPositions.Keys) unit.transform.position = _unitsPositions[unit];
        }

        public void UpdateDemarcation(Squad frontSquad)
        {
            if (!DemarcatesRounds) return;

            if (frontSquad == null || frontSquad.Count <= 0) return;
            _frontSquad = frontSquad;

            (bool isFound, Vector3 position) = GetRoundDemarcationPosition();
            RoundDemarcation.SetActive(isFound);
            if (!isFound) return;

            RoundDemarcation.transform.position = position;
        }

        private IEnumerator AnimateMoveDemarcation(float duration)
        {
            if (!DemarcatesRounds) yield break;

            (bool isFound, Vector3 position) = GetRoundDemarcationPosition();
            RoundDemarcation.SetActive(isFound);
            if (!isFound) yield break;

            yield return Mover.Move(transform: RoundDemarcation.transform, end: position, duration: duration);
        }

        private (bool isFound, Vector3 position) GetRoundDemarcationPosition()
        {
            if (!DemarcatesRounds) return (false, Vector3.negativeInfinity);

            Unit lastUnitInCycle = _frontSquad.BackUnit;
            if (lastUnitInCycle == null || !_unitsPositions.ContainsKey(lastUnitInCycle))
                return (false, Vector3.negativeInfinity);

            Vector3 position = RoundDemarcation.transform.position;
            return (true, new Vector3(
                x: _unitsPositions[lastUnitInCycle].x + 0.95f,
                y: position.y,
                z: position.z
            ));
        }

        private bool UpdateUnitsAndPositions(IEnumerable<Unit> units)
        {
            units = units.ToList();
            var hasChanged = false;

            var positionedUnits = 0;
            foreach (Unit unit in units)
            {
                Vector3 position = UnitsParent.TransformPoint(
                    new Vector3(
                        x: positionedUnits * UnitSpacingX * (IsInverted ? -1 : 1),
                        y: UnitStaggerY * (positionedUnits % 2),
                        z: 0f
                    )
                );
                positionedUnits++;

                if (_unitsPositions.ContainsKey(unit) && _unitsPositions[unit] == position) continue;

                unit.transform.SetParent(UnitsParent);
                unit.gameObject.SetActive(true);
                unit.transform.localScale = Vector3.one;

                _unitsPositions[unit] = position;

                hasChanged = true;
            }

            List<Unit> removedUnits = _unitsPositions.Keys.Except(units).ToList();
            if (removedUnits.Count > 0) hasChanged = true;
            foreach (Unit unit in removedUnits) _unitsPositions.Remove(unit);

            return hasChanged;
        }
    }
}
