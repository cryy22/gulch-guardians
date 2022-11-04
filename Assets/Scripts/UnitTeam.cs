using System.Collections.Generic;
using System.Linq;
using InfiniteSAPPrototype;
using UnityEngine;

public class UnitTeam : MonoBehaviour
{
    [SerializeField] private bool IsPlayerTeam;

    [SerializeField] public int UnitsPerCombatCycle = 3;
    [SerializeField] private int UnitCount = 3;
    [SerializeField] private bool DemarcatesRounds;
    [SerializeField] private UnitFactory UnitFactory;

    [SerializeField] private Transform UnitList;
    [SerializeField] private GameObject RoundDemarcation;

    public List<Unit> Units = new();

    public Unit FrontUnit => Units.Count > 0 ? Units.First() : null;
    public int UnitsInCombatCycle { get; private set; }

    private void Start() // should probably be Awake
    {
        RoundDemarcation.SetActive(DemarcatesRounds);

        foreach (Transform child in UnitList) Destroy(child.gameObject);

        for (var i = 0; i < UnitCount; i++)
        {
            Unit unit = UnitFactory.Create(isPlayerTeam: IsPlayerTeam);
            AddUnit(unit);
        }

        ResetUnitsOnDeck();
    }

    public void UnitDefeated()
    {
        Units.RemoveAt(0);
        UnitsInCombatCycle--;
        UpdateDemarcation();
    }

    public void ResetUnitsOnDeck()
    {
        UnitsInCombatCycle = Mathf.Min(a: UnitsPerCombatCycle, b: Units.Count);
        UpdateDemarcation();
    }

    public void AddUnit(Unit unit)
    {
        unit.transform.SetParent(UnitList);
        Units.Add(unit);
    }

    private void UpdateDemarcation()
    {
        if (!DemarcatesRounds) return;

        RoundDemarcation.SetActive(UnitsInCombatCycle > 0);
        if (UnitsInCombatCycle == 0) return;

        Transform lastUnitInCombatCycle = UnitList.GetChild(UnitsInCombatCycle - 1);

        Vector3 initialPosition = RoundDemarcation.transform.position;
        RoundDemarcation.transform.position = new Vector3(
            x: lastUnitInCombatCycle.position.x + 1f,
            y: initialPosition.y,
            z: initialPosition.z
        );
    }
}
