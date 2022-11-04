using System.Collections.Generic;
using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(fileName = "NewUnitConfig", menuName = "Scriptable Objects/Config/Unit Config")]
    public class UnitConfig : ScriptableObject
    {
        public List<Sprite> Sprites;
        public bool IsBoss;
        public int MinAttack;
        public int MaxAttack;
        public int MinHealth;
        public int MaxHealth;
    }
}
