using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GulchGuardians;
using UnityEngine;

[RequireComponent(typeof(Team))]
public class UnitsDisplayer : MonoBehaviour
{
    public float UnitSpacingX = 1.8825f;
    public float UnitStaggerY = 0.25f;
    public bool IsInverted;

    [SerializeField] private bool DemarcatesRounds;

    [SerializeField] private Transform UnitsParent;
    [SerializeField] private GameObject RoundDemarcation;

    private readonly Dictionary<Unit, Vector3> _unitsPositions = new();

    private void Awake() { RoundDemarcation.SetActive(DemarcatesRounds); }

    public IEnumerator AnimateUpdateDisplay(List<Unit> units)
    {
        bool hasChanged = UpdateUnitsAndPositions(units);
        if (!hasChanged) yield break;

        IEnumerable<Coroutine> coroutines = _unitsPositions.Keys.Select(
            unit => StartCoroutine(unit.AnimateToPosition(_unitsPositions[unit]))
        );
        foreach (Coroutine coroutine in coroutines) yield return coroutine;
    }

    public void UpdateDisplay(List<Unit> units)
    {
        UpdateUnitsAndPositions(units);
        foreach (Unit unit in _unitsPositions.Keys) unit.transform.position = _unitsPositions[unit];
    }

    public void UpdateDemarcation(Unit lastUnitInCycle, int unitsInCombatCycle)
    {
        if (!DemarcatesRounds) return;

        RoundDemarcation.SetActive(unitsInCombatCycle > 0);
        if (unitsInCombatCycle == 0) return;

        Vector3 initialPosition = RoundDemarcation.transform.position;
        RoundDemarcation.transform.position = new Vector3(
            x: lastUnitInCycle.transform.position.x + 0.95f,
            y: initialPosition.y,
            z: initialPosition.z
        );
    }

    private bool UpdateUnitsAndPositions(List<Unit> units)
    {
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
