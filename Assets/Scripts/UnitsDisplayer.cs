using System;
using GulchGuardians;
using UnityEngine;

[RequireComponent(typeof(Team))]
public class UnitsDisplayer : MonoBehaviour
{
    public float UnitSpacing = 1.8825f;
    public bool IsInverted;

    [SerializeField] private Transform UnitsParent;

    private Team _team;

    private void Awake() { _team = GetComponent<Team>(); }

    private void OnEnable()
    {
        _team.UnitsChanged += UnitsChangedEventHandler;
        UnitsChangedEventHandler(sender: this, e: EventArgs.Empty);
    }

    private void OnDisable() { _team.UnitsChanged -= UnitsChangedEventHandler; }

    private void UnitsChangedEventHandler(object sender, EventArgs e)
    {
        foreach (Unit unit in _team.Units) unit.gameObject.SetActive(false);
        UpdateUnitPositions();
    }

    private void UpdateUnitPositions()
    {
        var positionedUnits = 0;
        foreach (Unit unit in _team.Units)
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
}
