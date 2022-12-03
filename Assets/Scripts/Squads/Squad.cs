using System.Collections.Generic;
using Crysc.Initialization;
using GulchGuardians.Units;

namespace GulchGuardians.Squads
{
    public class Squad : InitializationBehaviour<SquadInitParams>
    {
        public IEnumerable<Unit> Units => InitParams.InitialUnits;
    }
}
