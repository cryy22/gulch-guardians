using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GulchGuardians
{
    public class Team : MonoBehaviour
    {
        public List<Unit> Units = new();
        public int UnitsPerCombatCycle = 3;

        [SerializeField] private bool IsPlayerTeam;
        [SerializeField] private int UnitCount = 3;

        [SerializeField] private UnitFactory UnitFactory;

        public event EventHandler UnitsChanged;

        public Unit FrontUnit => Units.Count > 0 ? Units.First() : null;
        public int UnitsInCombatCycle { get; private set; }

        private void Awake()
        {
            for (var i = 0; i < UnitCount; i++)
            {
                Unit unit = UnitFactory.Create(isPlayerTeam: IsPlayerTeam);
                AddUnit(unit);
            }

            ResetUnitsOnDeck();
        }

        public IEnumerator UnitDefeated(Unit unit)
        {
            if (!Units.Remove(unit)) yield break;

            yield return unit.BecomeDefeated();
            UnitsInCombatCycle--;
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void ResetUnitsOnDeck()
        {
            UnitsInCombatCycle = Mathf.Min(a: UnitsPerCombatCycle, b: Units.Count);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void AddUnit(Unit unit)
        {
            Units.Add(unit);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void SetUnitIndex(Unit unit, int index)
        {
            if (!Units.Remove(unit)) return;

            Units.Insert(index: index, item: unit);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }
    }
}
