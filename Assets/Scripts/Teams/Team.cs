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
            squad.Team = this;
            squad.transform.SetParent(parent: SquadsParent, worldPositionStays: false);
            if (IsUnitOrderInverted) squad.InvertArrangementOrder();

            _squads.Add(squad);
            squad.UnitsChanged += UnitsChangedEventHandler;
            squad.UnitClicked += UnitClickedEventHandler;
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
            // _unitsDisplayer.UpdateDemarcation(FrontSquad);
        }

        public IEnumerator AddUnit(Unit unit)
        {
            if (Units.Count() >= MaxUnits) throw new Exception("Team is full");
            yield return FrontSquad.AddUnit(unit);
        }

        public void HandleSquadDefeat(Squad squad)
        {
            _squads.Remove(squad);
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
