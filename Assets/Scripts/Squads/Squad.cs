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
    [RequireComponent(typeof(UISquad))]
    public class Squad : InitializationBehaviour<SquadInitParams>
    {
        private readonly List<Unit> _units = new();

        public event EventHandler UnitsChanged;
        public event EventHandler<UnitClickedEventArgs> UnitClicked;

        public IEnumerable<Unit> Units => _units;
        public int Count => _units.Count;
        public Unit FrontUnit => _units.Count > 0 ? _units.First() : null;
        public Unit BackUnit => _units.Count > 0 ? _units.Last() : null;

        public UISquad UI { get; private set; }
        public Team Team { get; set; }

        private void Awake() { UI = GetComponent<UISquad>(); }

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
            UI.Setup(this);
        }

        public IEnumerator HandleUnitDefeat(Unit unit)
        {
            if (!_units.Remove(unit)) yield break;

            UI.SetElements(Units.Select(u => u.UI));
            yield return UI.AnimateRearrange();

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);

            if (!IsDefeated(this)) yield break;
            yield return Team.HandleSquadDefeat(this);
            Destroy(gameObject);
        }

        public IEnumerator SetUnitIndex(Unit unit, int index, bool withHurtAnimation = false)
        {
            if (!_units.Remove(unit)) yield break;
            _units.Insert(index: index, item: unit);

            yield return UI.AnimateUpdateUnitIndex(units: Units, unit: unit, withHurtAnimation: withHurtAnimation);

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public IEnumerator AddUnit(Unit unit)
        {
            OnboardUnit(unit);
            UI.SetElements(Units.Select(u => u.UI));
            yield return UI.AnimateRearrange();

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

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
