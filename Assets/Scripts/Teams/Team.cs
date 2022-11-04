using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GulchGuardians
{
    [RequireComponent(typeof(UnitsDisplayer))]
    public class Team : MonoBehaviour
    {
        public List<Unit> Units = new();
        public int UnitsPerCombatCycle = 3;

        [SerializeField] private List<UnitSet> UnitSets;

        private UnitsDisplayer _unitsDisplayer;

        public event EventHandler UnitsChanged;
        public event EventHandler<UnitClickedEventArgs> UnitClicked;

        public Unit FrontUnit => Units.Count > 0 ? Units.First() : null;
        public int UnitsInCombatCycle { get; private set; }
        private Unit LastUnitInCycle => UnitsInCombatCycle > 0 ? Units[UnitsInCombatCycle - 1] : null;

        private void Awake()
        {
            _unitsDisplayer = GetComponent<UnitsDisplayer>();

            List<Unit> units = new();
            foreach (UnitSet unitSet in UnitSets) units.AddRange(unitSet.GenerateUnits());
            AddUnits(units);

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

        public void AddUnit(Unit unit)
        {
            AddUnitInternal(unit);

            _unitsDisplayer.UpdateDisplay(units: Units);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public IEnumerator DefeatUnit(Unit unit)
        {
            if (!Units.Remove(unit)) yield break;

            unit.GetComponent<ClickReporter>().OnReporterClickedEvent -= OnUnitClickedEventHandler;

            yield return unit.BecomeDefeated();
            UnitsInCombatCycle--;

            _unitsDisplayer.UpdateDisplay(units: Units);
            _unitsDisplayer.UpdateDemarcation(lastUnitInCycle: LastUnitInCycle, unitsInCombatCycle: UnitsInCombatCycle);

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void ResetUnitsOnDeck()
        {
            UnitsInCombatCycle = FrontUnit && FrontUnit.IsBoss
                ? 1
                : Mathf.Min(a: UnitsPerCombatCycle, b: Units.Count);

            _unitsDisplayer.UpdateDemarcation(lastUnitInCycle: LastUnitInCycle, unitsInCombatCycle: UnitsInCombatCycle);
        }

        public IEnumerator SetUnitIndex(Unit unit, int index)
        {
            if (!Units.Remove(unit)) yield break;

            Units.Insert(index: index, item: unit);

            yield return _unitsDisplayer.AnimateUpdateDisplay(units: Units);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private void AddUnits(IEnumerable<Unit> units)
        {
            foreach (Unit unit in units)
                AddUnitInternal(unit);

            _unitsDisplayer.UpdateDisplay(units: Units);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private void AddUnitInternal(Unit unit)
        {
            Units.Add(unit);
            unit.GetComponent<ClickReporter>().OnReporterClickedEvent += OnUnitClickedEventHandler;
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
