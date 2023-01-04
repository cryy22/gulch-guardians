using System;
using System.Collections.Generic;
using GulchGuardians.Abilities;
using GulchGuardians.Classes;

namespace GulchGuardians.Units
{
    [Serializable]
    public struct UnitInitParams
    {
        public string FirstName;
        public int Attack;
        public int Health;
        public int MaxHealth;
        public ClassType Class;
        public UnitSpriteAssetMap SpriteAssetMap;
        public IReadOnlyDictionary<AbilityType, bool> Abilities;

        public bool HasAbility(AbilityType ability) { return Abilities.GetValueOrDefault(ability); }
    }
}
