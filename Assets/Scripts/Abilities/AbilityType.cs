using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "New AbilityType", menuName = "Scriptable Objects/Types/Ability")]
    public class AbilityType : ScriptableObject
    {
        public string Name;
        [TextArea] public string Description;
        public Sprite Icon;
    }
}
