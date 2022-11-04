using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GulchGuardians;
using UnityEngine;

[RequireComponent(typeof(Team))]
public class UnitsDisplayer : MonoBehaviour
{
    public float UnitSpacing = 1.8825f;
    public bool IsInverted;

    [SerializeField] private bool DemarcatesRounds;

    [SerializeField] private Transform UnitsParent;
    [SerializeField] private GameObject RoundDemarcation;

    private readonly Dictionary<Unit, Vector3> _unitsPositions = new();

    private void Awake() { RoundDemarcation.SetActive(DemarcatesRounds); }

    public IEnumerator AnimateUpdateDisplay(List<Unit> units)
    {
        UpdateUnitsAndPositions(units);

        foreach (Unit unit in _unitsPositions.Keys)
            yield return unit.AnimateToPosition(_unitsPositions[unit]);
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

    private void UpdateUnitsAndPositions(List<Unit> units)
    {
        var positionedUnits = 0;
        foreach (Unit unit in units)
        {
            unit.transform.SetParent(UnitsParent);
            unit.gameObject.SetActive(true);
            unit.transform.localScale = Vector3.one;

            var localPosition = new Vector3(
                x: positionedUnits * UnitSpacing * (IsInverted ? -1 : 1),
                y: 0f,
                z: 0f
            );
            _unitsPositions[unit] = UnitsParent.TransformPoint(localPosition);

            positionedUnits++;
        }

        List<Unit> removedUnits = _unitsPositions.Keys.Except(units).ToList();
        foreach (Unit unit in removedUnits) _unitsPositions.Remove(unit);
    }
}
