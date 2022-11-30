using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GulchGuardians.Abilities;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Teams
{
    [RequireComponent(typeof(UIUnitsDisplayer))]
    public class Team : MonoBehaviour
    {
        public int MaxUnits = 99;
        public int UnitsPerCombatCycle = 3;

        [SerializeField] private List<UnitSet> UnitSets;
        [SerializeField] private AbilityType BossType;

        private readonly List<Unit> _units = new();
        private UIUnitsDisplayer _unitsDisplayer;

        public event EventHandler UnitsChanged;
        public event EventHandler<UnitClickedEventArgs> UnitClicked;

        public IReadOnlyList<Unit> Units => _units;
        public Unit FrontUnit => _units.Count > 0 ? _units.First() : null;

        public int UnitsInCombatCycle { get; private set; }
        private Unit LastUnitInCycle => UnitsInCombatCycle > 0 ? _units[UnitsInCombatCycle - 1] : null;

        private void Awake() { _unitsDisplayer = GetComponent<UIUnitsDisplayer>(); }

        private void Start()
        {
            List<Unit> units = new();
            foreach (UnitSet unitSet in UnitSets) units.AddRange(unitSet.GenerateUnits());
            AddUnits(units); // doing this in Start avoids OnEnable adding EventHandlers a second time

            ResetUnitsOnDeck();
        }

        private void OnEnable()
        {
            foreach (Unit unit in _units) unit.Clicked += OnUnitClickedEventHandler;
        }

        private void OnDisable()
        {
            foreach (Unit unit in _units) unit.Clicked -= OnUnitClickedEventHandler;
        }

        public IEnumerator AddUnit(Unit unit)
        {
            if (_units.Count >= MaxUnits) throw new Exception("Team is full");

            AddUnitInternal(unit);

            yield return _unitsDisplayer.AnimateUpdateDisplay(units: _units);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void ResetUnitsOnDeck()
        {
            UnitsInCombatCycle = FrontUnit && FrontUnit.HasAbility(BossType)
                ? 1
                : Mathf.Min(a: UnitsPerCombatCycle, b: _units.Count);

            _unitsDisplayer.UpdateDemarcation(LastUnitInCycle);
        }

        public IEnumerator SetUnitIndex(Unit unit, int index)
        {
            if (!_units.Remove(unit)) yield break;
            _units.Insert(index: index, item: unit);

            yield return _unitsDisplayer.AnimateUpdateDisplay(units: _units);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public IEnumerator HandleUnitDefeat(Unit unit)
        {
            if (!_units.Remove(unit)) yield break;
            UnitsInCombatCycle--;

            yield return _unitsDisplayer.AnimateUpdateDisplay(units: _units);

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private void AddUnits(IEnumerable<Unit> units)
        {
            if (_units.Count + units.Count() > MaxUnits) throw new Exception("Team is full");

            foreach (Unit unit in units) AddUnitInternal(unit);

            _unitsDisplayer.UpdateDisplay(units: _units);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private void AddUnitInternal(Unit unit)
        {
            _units.Add(unit);
            unit.Team = this;
            unit.Clicked += OnUnitClickedEventHandler;
        }

        private void OnUnitClickedEventHandler(object sender, EventArgs e)
        {
            UnitClicked?.Invoke(sender: this, e: new UnitClickedEventArgs(unit: (Unit) sender));
        }

        public class UnitClickedEventArgs : EventArgs
        {
            public UnitClickedEventArgs(Unit unit) { Unit = unit; }
            public Unit Unit { get; }
        }
    }
}
