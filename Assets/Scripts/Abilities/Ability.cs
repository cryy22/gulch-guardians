using GulchGuardians;
using UnityEngine;

namespace Abilities
{
    public abstract class Ability : ScriptableObject
    {
        public abstract string Name { get; }

        public virtual WillTakeDamageResult WillTakeDamage(Unit unit, int intendedDamage)
        {
            return new WillTakeDamageResult { Damage = intendedDamage, IsExhausted = false };
        }

        public struct WillTakeDamageResult
        {
            public int Damage;
            public bool IsExhausted;
        }
    }
}
