using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GulchGuardians.Squads;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Teams
{
    [RequireComponent(typeof(TeamView))]
    public class Team : MonoBehaviour
    {
        public int MaxUnits = 99;

        private readonly List<Squad> _squads = new();

        public event EventHandler UnitsChanged;
        public event EventHandler<UnitClickedEventArgs> UnitClicked;

        public IEnumerable<Squad> Squads => _squads;
        public Squad FrontSquad => _squads.Count > 0 ? _squads.First() : null;
        public IEnumerable<Unit> Units => _squads.SelectMany(s => s.Units);
        public bool IsDefeated => Units.Count() == 0;
        public TeamView View { get; private set; }

        private void Awake() { View = GetComponent<TeamView>(); }

        private void OnEnable()
        {
            foreach (Squad squad in _squads)
            {
                squad.UnitsChanged += UnitsChangedEventHandler;
                squad.UnitClicked += UnitClickedEventHandler;
            }
        }

        private void OnDisable()
        {
            foreach (Squad squad in _squads)
            {
                squad.UnitsChanged -= UnitsChangedEventHandler;
                squad.UnitClicked -= UnitClickedEventHandler;
            }
        }

        public void AddSquad(Squad squad)
        {
            _squads.Add(squad);
            squad.Team = this;

            if (gameObject.activeInHierarchy)
            {
                squad.UnitsChanged += UnitsChangedEventHandler;
                squad.UnitClicked += UnitClickedEventHandler;
            }

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
            View.UpdateArrangement();
            // _unitsDisplayer.UpdateDemarcation(FrontSquad);
        }

        public IEnumerator AddUnit(Unit unit)
        {
            if (Units.Count() >= MaxUnits) throw new Exception("Team is full");
            return FrontSquad.AddUnit(unit);
        }

        public IEnumerator HandleSquadDefeat(Squad squad)
        {
            _squads.Remove(squad);
            return View.AnimateUpdateArrangement();

            // _unitsDisplayer.UpdateDemarcation(FrontSquad);
        }

        private void UnitsChangedEventHandler(object sender, EventArgs e)
        {
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private void UnitClickedEventHandler(object sender, UnitClickedEventArgs e)
        {
            UnitClicked?.Invoke(sender: this, e: e);
        }
    }
}
