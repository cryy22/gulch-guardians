using GulchGuardians.Constants;
using UnityEngine;

namespace GulchGuardians.Units
{
    [CreateAssetMenu(fileName = "UnitFactory", menuName = "Factories/Unit Factory")]
    public class UnitFactory : ScriptableObject
    {
        [SerializeField] private Unit UnitPrefab;

        public Unit Create(UnitConfig config)
        {
            Unit unit = Instantiate(UnitPrefab);

            unit.Initialize(
                initParams: new UnitInitParams
                {
                    FirstName = Names.RandomName(),
                    Attack = Random.Range(minInclusive: config.MinAttack, maxExclusive: config.MaxAttack + 1),
                    Health = Random.Range(minInclusive: config.MinHealth, maxExclusive: config.MaxHealth + 1),
                    Abilities = config.Abilities,
                    SpriteLibraryAsset = config.GetSpriteLibraryAsset(),
                }
            );

            return unit;
        }
    }
}
