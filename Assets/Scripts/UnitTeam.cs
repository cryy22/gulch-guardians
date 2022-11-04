using System.Collections.Generic;
using System.Linq;
using InfiniteSAPPrototype;
using UnityEngine;

public class UnitTeam : MonoBehaviour
{
    [SerializeField] public int UnitsPerCombatCycle = 3;

    [SerializeField] private int UnitCount = 3;
    [SerializeField] private UnitFactory UnitFactory;

    public List<Unit> Units = new();

    public Unit FrontUnit => Units.Count > 0 ? Units.First() : null;
    public int UnitsInCombatCycle { get; private set; }

    private void Start()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);

        for (var i = 0; i < UnitCount; i++)
        {
            Unit unit = UnitFactory.Create();
            AddUnit(unit);
        }

        ResetUnitsOnDeck();
    }

    public void UnitDefeated()
    {
        Units.RemoveAt(0);
        UnitsInCombatCycle--;
    }

    public void ResetUnitsOnDeck() { UnitsInCombatCycle = Mathf.Min(a: UnitsPerCombatCycle, b: Units.Count); }

    public void AddUnit(Unit unit)
    {
        unit.transform.SetParent(transform);
        Units.Add(unit);
    }
}
