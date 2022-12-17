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
    public class Squad : InitializationBehaviour<SquadInitParams>
    {
        private readonly List<Unit> _units = new();

        private UIArrangement _arrangement;

        public IEnumerable<Unit> Units => _units;
        public int Count => _units.Count;
        public Unit FrontUnit => _units.Count > 0 ? _units.First() : null;
        public Unit BackUnit => _units.Count > 0 ? _units.Last() : null;
        public Team Team { get; set; }

        private IEnumerable<Transform> UnitTransforms => _units.Select(u => u.transform);

        private void Awake() { _arrangement = GetComponent<UIArrangement>(); }

        public static bool IsDefeated(Squad squad) { return squad == null || squad.Count <= 0; }

        public override void Initialize(SquadInitParams initParams)
        {
            base.Initialize(initParams);

            _units.AddRange(initParams.InitialUnits);
            foreach (Unit unit in _units) unit.Squad = this;
            _arrangement.UpdateElements(UnitTransforms);
        }

        public void InvertArrangementOrder() { _arrangement.InvertOrder(); }

        public IEnumerator HandleUnitDefeat(Unit unit)
        {
            if (!_units.Remove(unit)) yield break;
            yield return _arrangement.AnimateUpdateElements(UnitTransforms);
            Team.HandleUnitDefeat();

            if (!IsDefeated(this)) yield break;
            Team.HandleSquadDefeat(this);
            Destroy(gameObject);
        }

        public IEnumerator SetUnitIndex(Unit unit, int index, bool withHurtAnimation = false)
        {
            if (!_units.Remove(unit)) yield break;
            _units.Insert(index: index, item: unit);

            if (withHurtAnimation) unit.SetHurtAnimation();
            yield return _arrangement.AnimateUpdateElements(UnitTransforms);
            if (withHurtAnimation) unit.SetIdleAnimation();

            Team.SetUnitIndex();
        }

        public IEnumerator AddUnit(Unit unit)
        {
            unit.Squad = this;
            _units.Add(unit);

            yield return _arrangement.AnimateUpdateElements(UnitTransforms);
        }
    }
}
