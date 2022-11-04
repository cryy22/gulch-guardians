using UnityEngine;

namespace GulchGuardians
{
    [CreateAssetMenu(fileName = "NewUnitConfig", menuName = "Scriptable Objects/Config/Unit Config")]
    public class UnitConfig : ScriptableObject
    {
        public Sprite Sprite;
        public int MinAttack;
        public int MaxAttack;
        public int MinHealth;
        public int MaxHealth;
    }
}
