using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New AbilityInfo", menuName = "Scriptable Objects/Config/Ability Info")]
    public class AbilityInfo : ScriptableObject
    {
        public string Name;
        [TextArea] public string Description;
    }
}
