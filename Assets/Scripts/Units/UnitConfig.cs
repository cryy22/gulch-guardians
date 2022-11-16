using System.Collections.Generic;
using System.Linq;
using Abilities;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace GulchGuardians
{
    [CreateAssetMenu(fileName = "NewUnitConfig", menuName = "Config/Unit")]
    public class UnitConfig : ScriptableObject
    {
        public int MinAttack;
        public int MaxAttack;
        public int MinHealth;
        public int MaxHealth;

        public List<SpriteLibraryAsset> SpriteLibraryAssets;

        [SerializeField] private List<AbilityType> AbilityTypes;

        public IReadOnlyDictionary<AbilityType, bool> Abilities =>
            AbilityTypes.ToDictionary(a => a, _ => true);

        public SpriteLibraryAsset GetSpriteLibraryAsset()
        {
            return SpriteLibraryAssets[Random.Range(
                minInclusive: 0,
                maxExclusive: SpriteLibraryAssets.Count
            )];
        }
    }
}
