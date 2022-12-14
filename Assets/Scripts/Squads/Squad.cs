using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Initialization;
using GulchGuardians.Teams;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Squads
{
    [RequireComponent(typeof(SquadView))]
    public class Squad : InitializationBehaviour<SquadInitParams>
    {
        private readonly List<Unit> _units = new();

        public event EventHandler UnitsChanged;
        public event EventHandler<UnitClickedEventArgs> UnitClicked;

        public IEnumerable<Unit> Units => _units;
        public int Count => _units.Count;
        public Unit FrontUnit => _units.Count > 0 ? _units.First() : null;
        public Unit BackUnit => _units.Count > 0 ? _units.Last() : null;

        public SquadView View { get; private set; }
        public SquadReorderer Reorderer { get; private set; }
        public Team Team { get; set; }

        private void Awake()
        {
            View = GetComponent<SquadView>();
            Reorderer = GetComponent<SquadReorderer>();
        }

        private void OnEnable()
        {
            foreach (Unit unit in _units) unit.Clicked += UnitClickedEventHandler;
        }

        private void OnDisable()
        {
            foreach (Unit unit in _units) unit.Clicked -= UnitClickedEventHandler;
        }

        public static bool IsDefeated(Squad squad) { return squad == null || squad.Count <= 0; }

        public override void Initialize(SquadInitParams initParams)
        {
            base.Initialize(initParams);

            foreach (Unit unit in initParams.InitialUnits) OnboardUnit(unit);
            View.UpdateArrangement();
        }

        public IEnumerator HandleUnitDefeat(Unit unit)
        {
            if (!_units.Remove(unit)) yield break;

            yield return View.AnimateUpdateArrangement();

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);

            if (!IsDefeated(this)) yield break;
            yield return Team.HandleSquadDefeat(this);
            Destroy(gameObject);
        }

        public IEnumerator SetUnitIndex(Unit unit, int index, bool withHurtAnimation = false)
        {
            if (!_units.Remove(unit)) yield break;
            _units.Insert(index: index, item: unit);

            yield return View.AnimateUpdateUnitIndex(unit: unit, withHurtAnimation: withHurtAnimation);

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public IEnumerator AddUnit(Unit unit)
        {
            OnboardUnit(unit);

            yield return View.AnimateUpdateArrangement();

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public int GetUnitIndex(Unit unit) { return _units.IndexOf(item: unit); }

        private void OnboardUnit(Unit unit)
        {
            unit.Squad = this;
            unit.Clicked += UnitClickedEventHandler;

            _units.Add(unit);
        }

        private void UnitClickedEventHandler(object sender, EventArgs e)
        {
            UnitClicked?.Invoke(sender: this, e: new UnitClickedEventArgs(unit: (Unit) sender));
        }
    }
}
