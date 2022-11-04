using System;
using System.Collections;
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

    private Team _team;
    private bool _isUpdateEnqueued;

    private void Awake()
    {
        _team = GetComponent<Team>();
        RoundDemarcation.SetActive(DemarcatesRounds);
    }

    private void OnEnable()
    {
        _team.UnitsChanged += UnitsChangedEventHandler;
        UnitsChangedEventHandler(sender: this, e: EventArgs.Empty);
    }

    private void OnDisable() { _team.UnitsChanged -= UnitsChangedEventHandler; }

    private void UnitsChangedEventHandler(object sender, EventArgs e)
    {
        foreach (Unit unit in _team.Units) unit.gameObject.SetActive(false);
        if (_isUpdateEnqueued) return;

        StartCoroutine(UpdateUnitPositions());
        _isUpdateEnqueued = true;
    }

    private IEnumerator UpdateUnitPositions()
    {
        yield return new WaitForEndOfFrame();

        var positionedUnits = 0;
        foreach (Unit unit in _team.Units)
        {
            unit.gameObject.SetActive(true);
            unit.transform.SetParent(UnitsParent);
            unit.transform.localPosition = new Vector3(
                x: positionedUnits * UnitSpacing * (IsInverted ? -1 : 1),
                y: 0f,
                z: 0f
            );
            unit.transform.localScale = Vector3.one;

            positionedUnits++;
        }

        UpdateDemarcation();
        _isUpdateEnqueued = false;
    }

    private void UpdateDemarcation()
    {
        if (!DemarcatesRounds) return;

        RoundDemarcation.SetActive(_team.UnitsInCombatCycle > 0);
        if (_team.UnitsInCombatCycle == 0) return;

        Transform lastUnitInCombatCycle = _team.Units[_team.UnitsInCombatCycle - 1].transform;

        Vector3 initialPosition = RoundDemarcation.transform.position;
        RoundDemarcation.transform.position = new Vector3(
            x: lastUnitInCombatCycle.position.x + 1f,
            y: initialPosition.y,
            z: initialPosition.z
        );
    }
}
