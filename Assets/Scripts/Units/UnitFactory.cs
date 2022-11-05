using GulchGuardians.Constants;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(fileName = "UnitFactory", menuName = "Scriptable Objects/Factories/Unit Factory")]
    public class UnitFactory : ScriptableObject
    {
        [SerializeField] private Unit UnitPrefab;

        public Unit CreateFromConfig(UnitConfig config)
        {
            Unit unit = Instantiate(UnitPrefab);
            unit.Initialize(
                spriteLibraryAsset: config.GetSpriteLibraryAsset(),
                attributes: new Unit.Attributes
                {
                    FirstName = Name.RandomName(),
                    Attack = Random.Range(minInclusive: config.MinAttack, maxExclusive: config.MaxAttack + 1),
                    Health = Random.Range(minInclusive: config.MinHealth, maxExclusive: config.MaxHealth + 1),
                    IsBoss = config.IsBoss,
                    IsSturdy = config.IsSturdy,
                    IsArcher = Random.value > 0.5f,
                }
            );

            return unit;
        }
    }
}
