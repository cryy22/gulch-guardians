using UnityEngine;

namespace GulchGuardians.Abilities
{
    [CreateAssetMenu(fileName = "New AbilityType", menuName = "Types/Ability")]
    public class AbilityType : ScriptableObject
    {
        public string Name;
        [TextArea] public string Description;
        public Sprite Icon;
        public bool IsBadForSoloTeam;
        public int MaxPerTeam = 1;
    }
}
