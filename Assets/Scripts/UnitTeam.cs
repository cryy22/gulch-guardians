using UnityEngine;

public class UnitTeam : MonoBehaviour
{
    public Unit FrontUnit => transform.childCount > 0 ? transform.GetChild(0).GetComponent<Unit>() : null;

    public void AddUnit(Unit unit) { unit.transform.SetParent(transform); }
}
