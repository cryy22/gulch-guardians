using GulchGuardians.Squads;
using GulchGuardians.Units;

namespace GulchGuardians.Coordinators
{
    public struct AttackContext
    {
        public Unit Attacker;
        public Unit Defender;
        public Squad AttackerSquad;
        public Squad DefenderSquad;
    }
}
