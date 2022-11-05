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
    private Unit _lastUnitInCycle;

    private void Awake() { RoundDemarcation.SetActive(DemarcatesRounds); }

    public IEnumerator AnimateUpdateDisplay(List<Unit> units)
    {
        bool hasChanged = UpdateUnitsAndPositions(units);
        if (!hasChanged) yield break;

        List<Coroutine> coroutines = new();
        coroutines.Add(
            StartCoroutine(
                CoroutineHelper.RunConcurrently(
                    behaviours: _unitsPositions.Keys,
                    unit => unit.MoveToPosition(position: _unitsPositions[unit], duration: 0.25f)
                )
            )
        );
        if (DemarcatesRounds)
            coroutines.Add(
                StartCoroutine(
                    AnimateMoveDemarcation(position: GetRoundDemarcationPosition(), duration: 0.25f)
                )
            );

        foreach (Coroutine coroutine in coroutines) yield return coroutine;
    }

    public void UpdateDisplay(List<Unit> units)
    {
        UpdateUnitsAndPositions(units);
        foreach (Unit unit in _unitsPositions.Keys) unit.transform.position = _unitsPositions[unit];
    }

    public void UpdateDemarcation(Unit lastUnitInCycle)
    {
        if (!DemarcatesRounds) return;

        if (!_unitsPositions.ContainsKey(lastUnitInCycle))
        {
            RoundDemarcation.SetActive(false);
            return;
        }

        RoundDemarcation.SetActive(true);
        _lastUnitInCycle = lastUnitInCycle;

        RoundDemarcation.transform.position = GetRoundDemarcationPosition();
    }

    private IEnumerator AnimateMoveDemarcation(Vector3 position, float duration)
    {
        if (!_unitsPositions.ContainsKey(_lastUnitInCycle))
        {
            RoundDemarcation.SetActive(false);
            yield break;
        }

        Vector3 startPosition = RoundDemarcation.transform.position;
        var t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / duration;
            RoundDemarcation.transform.position = Vector3.Lerp(a: startPosition, b: position, t: t);
            yield return null;
        }
    }

    private Vector3 GetRoundDemarcationPosition()
    {
        if (!_unitsPositions.ContainsKey(_lastUnitInCycle)) return Vector3.negativeInfinity;

        Vector3 position = RoundDemarcation.transform.position;
        return new Vector3(
            x: _unitsPositions[_lastUnitInCycle].x + 0.95f,
            y: position.y,
            z: position.z
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
