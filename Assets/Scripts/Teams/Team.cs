using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GulchGuardians.Squads;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Teams
{
    public class Team : MonoBehaviour
    {
        public int MaxUnits = 99;

        [SerializeField] private Transform SquadsParent;
        [SerializeField] private bool IsUnitOrderInverted;

        private readonly List<Squad> _squads = new();

        public event EventHandler UnitsChanged;
        public event EventHandler<UnitClickedEventArgs> UnitClicked;

        public IEnumerable<Unit> Units => _squads.SelectMany(s => s.Units);
        public Squad FrontSquad => _squads.Count > 0 ? _squads.First() : null;
        public bool IsDefeated => Units.Count() == 0;

        private void OnEnable()
        {
            foreach (Unit unit in Units) unit.Clicked += OnUnitClickedEventHandler;
        }

        private void OnDisable()
        {
            foreach (Unit unit in Units) unit.Clicked -= OnUnitClickedEventHandler;
        }

        public void AddSquad(Squad squad)
        {
            squad.Team = this;
            squad.transform.SetParent(parent: SquadsParent, worldPositionStays: false);
            if (IsUnitOrderInverted) squad.InvertArrangementOrder();

            _squads.Add(squad);
            AddUnits(squad.Units);
            // _unitsDisplayer.UpdateDemarcation(FrontSquad);
        }

        public IEnumerator AddUnit(Unit unit)
        {
            if (Units.Count() >= MaxUnits) throw new Exception("Team is full");

            yield return FrontSquad.AddUnit(unit);
            AddUnitInternal(unit);

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void SetUnitIndex() { UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty); }

        public void HandleUnitDefeat() { UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty); }

        public void HandleSquadDefeat(Squad squad)
        {
            _squads.Remove(squad);
            // _unitsDisplayer.UpdateDemarcation(FrontSquad);
        }

        private void AddUnits(IEnumerable<Unit> units)
        {
            List<Unit> enumeratedUnits = units.ToList();
            if (Units.Count() > MaxUnits) throw new Exception("Team is full");

            foreach (Unit unit in enumeratedUnits) AddUnitInternal(unit);

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private void AddUnitInternal(Unit unit) { unit.Clicked += OnUnitClickedEventHandler; }

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
