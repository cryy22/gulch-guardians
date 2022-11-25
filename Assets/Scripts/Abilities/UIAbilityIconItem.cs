using UnityEngine;
using UnityEngine.UI;

namespace GulchGuardians.Abilities
{
    public class UIAbilityIconItem : MonoBehaviour, IAbilityProvider
    {
        [SerializeField] private Image IconImage;

        public void Awake() { SetAbility(null); }

        public void SetAbility(AbilityType ability)
        {
            Ability = ability;
            IconImage.sprite = ability != null ? ability.Icon : null;
        }

        public AbilityType Ability { get; private set; }
    }
}
