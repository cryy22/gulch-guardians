using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace GulchGuardians
{
    [CreateAssetMenu(fileName = "NewUnitConfig", menuName = "Scriptable Objects/Config/Unit Config")]
    public class UnitConfig : ScriptableObject
    {
        public int MinAttack;
        public int MaxAttack;
        public int MinHealth;
        public int MaxHealth;
        public bool IsBoss;
        public bool IsSturdy;
        public bool IsArcher;

        public List<SpriteLibraryAsset> SpriteLibraryAssets;

        public SpriteLibraryAsset GetSpriteLibraryAsset()
        {
            return SpriteLibraryAssets[Random.Range(
                minInclusive: 0,
                maxExclusive: SpriteLibraryAssets.Count
            )];
        }
    }
}
