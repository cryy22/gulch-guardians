using GulchGuardians.Squads;
using GulchGuardians.Units;

namespace GulchGuardians.Coordination.Battle
{
    public struct ActionContext
    {
        public readonly Unit Actor;
        public readonly Squad AttackingSquad;
        public readonly Squad DefendingSquad;

        public ActionContext(Unit actor, Squad attackingSquad, Squad defendingSquad)
        {
            Actor = actor;
            AttackingSquad = attackingSquad;
            DefendingSquad = defendingSquad;
        }
    }
}
