using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Initialization;
using Crysc.UI;
using GulchGuardians.Teams;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Squads
{
    [RequireComponent(typeof(UIArrangement))]
    public class Squad : InitializationBehaviour<SquadInitParams>, IArrangementElement
    {
        private readonly List<Unit> _units = new();

        private UIArrangement _arrangement;

        public event EventHandler UnitsChanged;
        public event EventHandler<UnitClickedEventArgs> UnitClicked;

        public IEnumerable<Unit> Units => _units;
        public int Count => _units.Count;
        public Unit FrontUnit => _units.Count > 0 ? _units.First() : null;
        public Unit BackUnit => _units.Count > 0 ? _units.Last() : null;
        public Team Team { get; set; }

        private void Awake() { _arrangement = GetComponent<UIArrangement>(); }

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

            _units.AddRange(initParams.InitialUnits);
            foreach (Unit unit in _units) ConfigureUnit(unit);
            _arrangement.UpdateElements(Units);
        }

        public void InvertArrangementOrder() { _arrangement.InvertOrder(); }

        public IEnumerator HandleUnitDefeat(Unit unit)
        {
            if (!_units.Remove(unit)) yield break;
            yield return _arrangement.AnimateUpdateElements(Units);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);

            if (!IsDefeated(this)) yield break;
            yield return Team.HandleSquadDefeat(this);
            Destroy(gameObject);
        }

        public IEnumerator SetUnitIndex(Unit unit, int index, bool withHurtAnimation = false)
        {
            if (!_units.Remove(unit)) yield break;
            _units.Insert(index: index, item: unit);

            if (withHurtAnimation) unit.SetHurtAnimation();
            yield return _arrangement.AnimateUpdateElements(Units);
            if (withHurtAnimation) unit.SetIdleAnimation();

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public IEnumerator AddUnit(Unit unit)
        {
            ConfigureUnit(unit);
            _units.Add(unit);

            yield return _arrangement.AnimateUpdateElements(Units);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private void ConfigureUnit(Unit unit)
        {
            unit.Squad = this;
            unit.Clicked += UnitClickedEventHandler;
        }

        private void UnitClickedEventHandler(object sender, EventArgs e)
        {
            UnitClicked?.Invoke(sender: this, e: new UnitClickedEventArgs(unit: (Unit) sender));
        }

        // IArrangementElement
        public Transform Transform => transform;
        public Bounds Bounds => _arrangement.Bounds;
    }
}
