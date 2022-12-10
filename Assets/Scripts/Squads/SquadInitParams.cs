using System.Collections.Generic;
using GulchGuardians.Units;

namespace GulchGuardians.Squads
{
    public struct SquadInitParams
    {
        public readonly IEnumerable<Unit> InitialUnits;
        public readonly bool UnitsRotate;

        public SquadInitParams(IEnumerable<Unit> initialUnits, bool unitsRotate)
        {
            InitialUnits = initialUnits;
            UnitsRotate = unitsRotate;
        }
    }
}
