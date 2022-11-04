using UnityEngine;

public class UnitTeam : MonoBehaviour
{
    public Unit FrontUnit => transform.GetChild(0).GetComponent<Unit>();

    public void AddUnit(Unit unit) { unit.transform.SetParent(transform); }
}
