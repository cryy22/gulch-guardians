using System.Collections.Generic;
using System.Linq;
using GulchGuardians.Abilities;
using UnityEngine;

namespace GulchGuardians.Units
{
    [CreateAssetMenu(fileName = "NewUnitConfig", menuName = "Config/Unit")]
    public class UnitConfig : ScriptableObject
    {
        public int MinAttack;
        public int MaxAttack;
        public int MinHealth;
        public int MaxHealth;

        public List<UnitSpriteAssetMap> SpriteAssetMaps;

        [SerializeField] private List<AbilityType> AbilityTypes;

        public IReadOnlyDictionary<AbilityType, bool> Abilities =>
            AbilityTypes.ToDictionary(a => a, _ => true);

        public UnitSpriteAssetMap GetSpriteAssetMap()
        {
            return SpriteAssetMaps[Random.Range(
                minInclusive: 0,
                maxExclusive: SpriteAssetMaps.Count
            )];
        }
    }
}
