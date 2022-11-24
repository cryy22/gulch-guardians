using Crysc.Initialization;
using GulchGuardians.Constants;
using UnityEngine;

namespace GulchGuardians.Units
{
    [CreateAssetMenu(fileName = "UnitFactory", menuName = "Factories/Unit Factory")]
    public class UnitFactory : InitializationFactory<Unit, UnitConfig, UnitInitParams>
    {
        protected override UnitInitParams GetInitParams(UnitConfig config)
        {
            return new UnitInitParams
            {
                FirstName = Names.RandomName(),
                Attack = Random.Range(minInclusive: config.MinAttack, maxExclusive: config.MaxAttack + 1),
                Health = Random.Range(minInclusive: config.MinHealth, maxExclusive: config.MaxHealth + 1),
                Abilities = config.Abilities,
                SpriteAssetMap = config.GetSpriteAssetMap(),
            };
        }
    }
}
