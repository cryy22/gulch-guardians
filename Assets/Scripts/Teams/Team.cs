using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GulchGuardians.Squads;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Teams
{
    [RequireComponent(typeof(UIUnitsDisplayer))]
    public class Team : MonoBehaviour
    {
        public int MaxUnits = 99;

        [SerializeField] private List<SquadConfig> SquadConfigs;
        [SerializeField] private SquadFactory SquadFactory;

        private readonly List<Squad> _squads = new();
        private UIUnitsDisplayer _unitsDisplayer;

        public event EventHandler UnitsChanged;
        public event EventHandler<UnitClickedEventArgs> UnitClicked;

        public IEnumerable<Unit> Units => _squads.SelectMany(s => s.Units);
        public Squad FrontSquad => _squads.Count > 0 ? _squads.First() : null;

        private void Awake() { _unitsDisplayer = GetComponent<UIUnitsDisplayer>(); }

        private void Start()
        {
            foreach (SquadConfig squadConfig in SquadConfigs)
            {
                Squad squad = SquadFactory.Create(squadConfig);
                squad.Team = this;

                _squads.Add(squad);
            }

            AddUnits(Units); // doing this in Start avoids OnEnable adding EventHandlers a second time
            ResetUnitsOnDeck();
        }

        private void OnEnable()
        {
            foreach (Unit unit in Units) unit.Clicked += OnUnitClickedEventHandler;
        }

        private void OnDisable()
        {
            foreach (Unit unit in Units) unit.Clicked -= OnUnitClickedEventHandler;
        }

        public IEnumerator AddUnit(Unit unit)
        {
            if (Units.Count() >= MaxUnits) throw new Exception("Team is full");

            FrontSquad.AddUnit(unit);
            AddUnitInternal(unit);

            yield return _unitsDisplayer.AnimateUpdateDisplay(units: Units);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void ResetUnitsOnDeck() { _unitsDisplayer.UpdateDemarcation(FrontSquad); }

        public IEnumerator SetUnitIndex(Unit unit, bool withHurtAnimation = false)
        {
            if (withHurtAnimation) unit.SetHurtAnimation();
            yield return _unitsDisplayer.AnimateUpdateDisplay(units: Units);
            if (withHurtAnimation) unit.SetIdleAnimation();

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public IEnumerator HandleUnitDefeat(Unit unit)
        {
            yield return _unitsDisplayer.AnimateUpdateDisplay(units: Units);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void HandleSquadDefeat(Squad squad)
        {
            _squads.Remove(squad);
            _unitsDisplayer.UpdateDemarcation(FrontSquad);
        }

        private void AddUnits(IEnumerable<Unit> units)
        {
            IEnumerable<Unit> enumeratedUnits = units.ToArray();
            if (Units.Count() + enumeratedUnits.Count() > MaxUnits) throw new Exception("Team is full");

            foreach (Unit unit in enumeratedUnits) AddUnitInternal(unit);

            _unitsDisplayer.UpdateDisplay(units: Units);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private void AddUnitInternal(Unit unit)
        {
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
