using System;
using System.Collections.Generic;
using System.Linq;
using GulchGuardians.Abilities;

namespace GulchGuardians.Units
{
    [Serializable]
    public struct UnitInitParams
    {
        public string FirstName;
        public int Attack;
        public int Health;
        public int MaxHealth;
        public UnitSpriteAssetMap SpriteAssetMap;
        public IReadOnlyDictionary<AbilityType, bool> Abilities;

        public IEnumerable<AbilityType> ActiveAbilities =>
            Abilities.Where(pair => pair.Value).Select(pair => pair.Key);

        public bool HasAbility(AbilityType ability) { return Abilities.GetValueOrDefault(ability); }
    }
}
