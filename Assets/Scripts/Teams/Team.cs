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
        public event EventHandler<UnitClickedEventArgs> UnitClicked;

        public Unit FrontUnit => Units.Count > 0 ? Units.First() : null;
        public int UnitsInCombatCycle { get; private set; }

        private void Awake()
        {
            for (var i = 0; i < UnitCount; i++)
            {
                Unit unit = UnitFactory.Create(isPlayerTeam: IsPlayerTeam);
                AddUnit(unit: unit, skipClickHandling: true);
            }

            ResetUnitsOnDeck();
        }

        private void OnEnable()
        {
            foreach (Unit unit in Units)
                unit.GetComponent<ClickReporter>().OnReporterClickedEvent += OnUnitClickedEventHandler;
        }

        private void OnDisable()
        {
            foreach (Unit unit in Units)
                unit.GetComponent<ClickReporter>().OnReporterClickedEvent -= OnUnitClickedEventHandler;
        }

        public IEnumerator UnitDefeated(Unit unit)
        {
            if (!Units.Remove(unit)) yield break;
            unit.GetComponent<ClickReporter>().OnReporterClickedEvent -= OnUnitClickedEventHandler;
            UnitsInCombatCycle--;

            yield return unit.BecomeDefeated();

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void ResetUnitsOnDeck()
        {
            UnitsInCombatCycle = Mathf.Min(a: UnitsPerCombatCycle, b: Units.Count);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void AddUnit(Unit unit, bool skipClickHandling = false)
        {
            Units.Add(unit);
            if (!skipClickHandling)
                unit.GetComponent<ClickReporter>().OnReporterClickedEvent += OnUnitClickedEventHandler;

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void SetUnitIndex(Unit unit, int index)
        {
            if (!Units.Remove(unit)) return;

            Units.Insert(index: index, item: unit);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private void OnUnitClickedEventHandler(ClickReporter reporter)
        {
            UnitClicked?.Invoke(sender: this, e: new UnitClickedEventArgs(unit: reporter.GetComponent<Unit>()));
        }

        public class UnitClickedEventArgs : EventArgs
        {
            public UnitClickedEventArgs(Unit unit) { Unit = unit; }

            public Unit Unit { get; }
        }
    }
}
