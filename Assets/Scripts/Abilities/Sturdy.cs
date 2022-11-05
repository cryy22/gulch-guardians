using GulchGuardians;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "Sturdy", menuName = "Scriptable Objects/Abilities/Sturdy")]
    public class Sturdy : Ability
    {
        private const string _name = "Sturdy";
        public override string Name => _name;

        public override WillTakeDamageResult WillTakeDamage(Unit unit, int intendedDamage)
        {
            if (intendedDamage < unit.Health) return base.WillTakeDamage(unit: unit, intendedDamage: intendedDamage);

            return new WillTakeDamageResult
            {
                Damage = unit.Health - 1,
                IsExhausted = true,
            };
        }
    }
}
