using System.Collections.Generic;
using GulchGuardians.Units;

namespace GulchGuardians.Squads
{
    public struct SquadInitParams
    {
        public readonly IEnumerable<Unit> InitialUnits;

        public SquadInitParams(IEnumerable<Unit> initialUnits) { InitialUnits = initialUnits; }
    }
}
