using TMPro;
using UnityEngine;

namespace GulchGuardians.Abilities
{
    public class AbilityTextItem : MonoBehaviour, IAbilityProvider
    {
        [SerializeField] private TMP_Text Name;

        public void SetAbility(AbilityType ability)
        {
            Ability = ability;
            Name.text = ability != null ? ability.Name : "----";
        }

        // IAbilityProvider
        public AbilityType Ability { get; private set; }
    }
}
