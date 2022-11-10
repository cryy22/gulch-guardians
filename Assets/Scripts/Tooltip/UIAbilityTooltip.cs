using Abilities;
using TMPro;
using UnityEngine;

namespace Tooltip
{
    public class UIAbilityTooltip : MonoBehaviour
    {
        [SerializeField] private TMP_Text TitleText;
        [SerializeField] private TMP_Text DescriptionText;

        public void SetAbility(AbilityType ability)
        {
            TitleText.text = ability.Name;
            DescriptionText.text = ability.Description;
        }

        public void Show() { gameObject.SetActive(true); }

        public void Hide() { gameObject.SetActive(false); }
    }
}
