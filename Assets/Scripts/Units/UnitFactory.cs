using System.Collections.Generic;
using Abilities;
using GulchGuardians.Constants;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(fileName = "UnitFactory", menuName = "Scriptable Objects/Factories/Unit Factory")]
    public class UnitFactory : ScriptableObject
    {
        [SerializeField] private Unit UnitPrefab;

        [SerializeField] private AbilityType ArcherType;
        [SerializeField] private AbilityType BossType;
        [SerializeField] private AbilityType SturdyType;

        public Unit CreateFromConfig(UnitConfig config)
        {
            Unit unit = Instantiate(UnitPrefab);

            Dictionary<AbilityType, bool> abilities = new();
            if (config.IsArcher) abilities[ArcherType] = true;
            if (config.IsBoss) abilities[BossType] = true;
            if (config.IsSturdy) abilities[SturdyType] = true;

            unit.Initialize(
                spriteLibraryAsset: config.GetSpriteLibraryAsset(),
                attributes: new Unit.Attributes
                {
                    FirstName = Name.RandomName(),
                    Attack = Random.Range(minInclusive: config.MinAttack, maxExclusive: config.MaxAttack + 1),
                    Health = Random.Range(minInclusive: config.MinHealth, maxExclusive: config.MaxHealth + 1),
                    Abilities = abilities,
                }
            );

            return unit;
        }
    }
}
