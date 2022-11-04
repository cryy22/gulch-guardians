using DefaultNamespace;
using UnityEngine;

public class UnitTeam : MonoBehaviour
{
    [SerializeField] private int NumberOfUnits = 3;
    [SerializeField] private UnitFactory UnitFactory;

    public Unit FrontUnit => transform.childCount > 0 ? transform.GetChild(0).GetComponent<Unit>() : null;

    private void Start()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);

        for (var i = 0; i < NumberOfUnits; i++) AddUnit(UnitFactory.Create());
        Debug.Log(transform.childCount);
    }

    public void AddUnit(Unit unit) { unit.transform.SetParent(transform); }
}
