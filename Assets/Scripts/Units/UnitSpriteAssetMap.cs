using System;
using GulchGuardians.Classes;
using UnityEngine.U2D.Animation;

namespace GulchGuardians.Units
{
    [Serializable]
    public struct UnitSpriteAssetMap
    {
        public SpriteLibraryAsset Default;
        public SpriteLibraryAsset Healer;

        public SpriteLibraryAsset GetAsset(ClassType @class)
        {
            SpriteLibraryAsset asset = null;

            if (@class == ClassIndex.I.Healer) asset = Healer;
            else if (@class == ClassIndex.I.Rookie) asset = Default;

            return asset != null ? asset : Default;
        }
    }
}
