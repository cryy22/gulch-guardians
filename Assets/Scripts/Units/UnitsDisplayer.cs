using System.Collections.Generic;
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

    private void Awake() { RoundDemarcation.SetActive(DemarcatesRounds); }

    public void UpdateDisplay(List<Unit> units)
    {
        var positionedUnits = 0;
        foreach (Unit unit in units)
        {
            unit.transform.SetParent(UnitsParent);
            unit.transform.localPosition = new Vector3(
                x: positionedUnits * UnitSpacing * (IsInverted ? -1 : 1),
                y: 0f,
                z: 0f
            );
            unit.transform.localScale = Vector3.one;
            unit.gameObject.SetActive(true);

            positionedUnits++;
        }
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
}
