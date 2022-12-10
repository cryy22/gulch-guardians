using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Initialization;
using GulchGuardians.Teams;
using GulchGuardians.Units;

namespace GulchGuardians.Squads
{
    public class Squad : InitializationBehaviour<SquadInitParams>
    {
        private readonly List<Unit> _units = new();

        public IEnumerable<Unit> Units => _units;
        public int Count => _units.Count;
        public Unit FrontUnit => _units.Count > 0 ? _units.First() : null;
        public Unit BackUnit => _units.Count > 0 ? _units.Last() : null;
        public bool UnitsRotate => InitParams.UnitsRotate;
        public Team Team { get; set; }

        public static bool IsDefeated(Squad squad) { return squad == null || squad.Count <= 0; }

        public override void Initialize(SquadInitParams initParams)
        {
            base.Initialize(initParams);
            _units.AddRange(initParams.InitialUnits);
            foreach (Unit unit in _units) unit.Squad = this;
        }

        public IEnumerator HandleUnitDefeat(Unit unit)
        {
            if (!_units.Remove(unit)) yield break;
            yield return Team.HandleUnitDefeat(unit);

            if (!IsDefeated(this)) yield break;
            Team.HandleSquadDefeat(this);
            Destroy(gameObject);
        }

        public IEnumerator SetUnitIndex(Unit unit, int index, bool withHurtAnimation = false)
        {
            if (!_units.Remove(unit)) yield break;
            _units.Insert(index: index, item: unit);

            yield return Team.SetUnitIndex(unit: unit, withHurtAnimation: withHurtAnimation);
        }

        public void AddUnit(Unit unit)
        {
            unit.Squad = this;
            _units.Add(unit);
        }
    }
}
