using Crysc.Initialization;
using GulchGuardians.Classes;
using UnityEngine;

namespace GulchGuardians.Units
{
    [CreateAssetMenu(fileName = "UnitFactory", menuName = "Factories/Unit Factory")]
    public class UnitFactory : InitializationFactory<Unit, UnitConfig, UnitInitParams>
    {
        [SerializeField] private ClassIndex ClassIndex;

        protected override UnitInitParams GetInitParams(UnitConfig config)
        {
            return new UnitInitParams
            {
                FirstName = "Colin",
                Attack = Random.Range(minInclusive: config.MinAttack, maxExclusive: config.MaxAttack + 1),
                Health = Random.Range(minInclusive: config.MinHealth, maxExclusive: config.MaxHealth + 1),
                Abilities = config.Abilities,
                Class = ClassIndex.Rookie,
                SpriteAssetMap = config.GetSpriteAssetMap(),
            };
        }
    }
}
