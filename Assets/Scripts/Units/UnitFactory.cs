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

        public Unit CreateFromConfig(UnitConfig config)
        {
            Unit unit = Instantiate(UnitPrefab);

            List<Ability> abilities = new();
            if (config.IsArcher) abilities.Add(Ability.Archer);
            if (config.IsBoss) abilities.Add(Ability.Boss);
            if (config.IsSturdy) abilities.Add(Ability.Sturdy);

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
