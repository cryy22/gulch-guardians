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
        public int MaxUnits = 99;
        public int UnitsPerCombatCycle = 3;

        [SerializeField] private List<UnitSet> UnitSets;

        private UnitsDisplayer _unitsDisplayer;

        public event EventHandler UnitsChanged;
        public event EventHandler<UnitClickedEventArgs> UnitClicked;
        public Unit FrontUnit => Units.Count > 0 ? Units.First() : null;

        public bool IsReady { get; private set; } = true;
        public int UnitsInCombatCycle { get; private set; }
        private Unit LastUnitInCycle => UnitsInCombatCycle > 0 ? Units[UnitsInCombatCycle - 1] : null;

        private void Awake() { _unitsDisplayer = GetComponent<UnitsDisplayer>(); }

        private void Start()
        {
            List<Unit> units = new();
            foreach (UnitSet unitSet in UnitSets) units.AddRange(unitSet.GenerateUnits());
            AddUnits(units); // doing this in Start avoids OnEnable adding DefeatedEventHandler a second time

            ResetUnitsOnDeck();
        }

        private void OnEnable()
        {
            foreach (Unit unit in Units)
            {
                unit.GetComponent<ClickReporter>().OnReporterClickedEvent += OnUnitClickedEventHandler;
                unit.Defeated += DefeatedEventHandler;
            }
        }

        private void OnDisable()
        {
            foreach (Unit unit in Units)
            {
                unit.GetComponent<ClickReporter>().OnReporterClickedEvent -= OnUnitClickedEventHandler;
                unit.Defeated -= DefeatedEventHandler;
            }
        }

        public void AddUnit(Unit unit)
        {
            if (Units.Count >= MaxUnits) throw new Exception("Team is full");

            AddUnitInternal(unit);

            _unitsDisplayer.UpdateDisplay(units: Units);
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
            IsReady = false;

            Units.Insert(index: index, item: unit);

            yield return _unitsDisplayer.AnimateUpdateDisplay(units: Units);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);

            IsReady = true;
        }

        private void DefeatedEventHandler(object sender, EventArgs e)
        {
            StartCoroutine(DefeatUnit(unit: (Unit) sender));
        }

        private IEnumerator DefeatUnit(Unit unit)
        {
            if (!Units.Remove(unit)) yield break;
            IsReady = false;

            unit.GetComponent<ClickReporter>().OnReporterClickedEvent -= OnUnitClickedEventHandler;
            unit.Defeated -= DefeatedEventHandler;
            UnitsInCombatCycle--;

            yield return _unitsDisplayer.AnimateUpdateDisplay(units: Units);
            _unitsDisplayer.UpdateDemarcation(lastUnitInCycle: LastUnitInCycle, unitsInCombatCycle: UnitsInCombatCycle);

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);

            IsReady = true;
        }

        private void AddUnits(IEnumerable<Unit> units)
        {
            if (Units.Count + units.Count() > MaxUnits) throw new Exception("Team is full");

            foreach (Unit unit in units) AddUnitInternal(unit);

            _unitsDisplayer.UpdateDisplay(units: Units);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private void AddUnitInternal(Unit unit)
        {
            Units.Add(unit);
            unit.GetComponent<ClickReporter>().OnReporterClickedEvent += OnUnitClickedEventHandler;
            unit.Defeated += DefeatedEventHandler;
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
