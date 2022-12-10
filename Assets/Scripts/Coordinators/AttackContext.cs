using GulchGuardians.Squads;
using GulchGuardians.Units;

namespace GulchGuardians.Coordinators
{
    public struct AttackContext
    {
        public readonly Unit Actor;
        public readonly Unit Defender;
        public readonly Squad AttackingSquad;
        public readonly Squad DefendingSquad;

        public AttackContext(Unit actor, Unit defender, Squad attackingSquad, Squad defendingSquad)
        {
            Actor = actor;
            Defender = defender;
            AttackingSquad = attackingSquad;
            DefendingSquad = defendingSquad;
        }
    }
}
