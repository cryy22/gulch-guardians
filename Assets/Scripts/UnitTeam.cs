using DefaultNamespace;
using UnityEngine;

public class UnitTeam : MonoBehaviour
{
    [SerializeField] public int UnitsPerCombatCycle = 3;

    [SerializeField] private int UnitCount = 3;
    [SerializeField] private UnitFactory UnitFactory;

    public Unit FrontUnit => transform.childCount > 0 ? transform.GetChild(0).GetComponent<Unit>() : null;
    public int UnitsInCombatCycle { get; private set; }

    private void Start()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);

        for (var i = 0; i < UnitCount; i++)
        {
            Unit unit = UnitFactory.Create();
            unit.transform.SetParent(transform);
        }

        ResetUnitsOnDeck();
    }

    public void UnitDefeated()
    {
        UnitCount--;
        UnitsInCombatCycle--;
    }

    public void ResetUnitsOnDeck() { UnitsInCombatCycle = Mathf.Min(a: UnitsPerCombatCycle, b: UnitCount); }

    private void AddUnit(Unit unit)
    {
        unit.transform.SetParent(transform);
        UnitCount++;
    }
}
