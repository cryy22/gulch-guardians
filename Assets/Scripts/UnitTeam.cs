using System.Collections.Generic;
using UnityEngine;

public class UnitTeam : MonoBehaviour
{
    [SerializeField] private List<Unit> Units;

    public Unit FrontUnit(Unit unit) { return transform.GetChild(0).GetComponent<Unit>(); }
}
