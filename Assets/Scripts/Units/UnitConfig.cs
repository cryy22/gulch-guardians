using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace GulchGuardians
{
    [CreateAssetMenu(fileName = "NewUnitConfig", menuName = "Scriptable Objects/Config/Unit Config")]
    public class UnitConfig : ScriptableObject
    {
        public List<SpriteLibraryAsset> SpriteLibraryAssets;
        public bool IsBoss;
        public int MinAttack;
        public int MaxAttack;
        public int MinHealth;
        public int MaxHealth;

        public SpriteLibraryAsset GetSpriteLibraryAsset()
        {
            return SpriteLibraryAssets[Random.Range(
                minInclusive: 0,
                maxExclusive: SpriteLibraryAssets.Count
            )];
        }
    }
}
