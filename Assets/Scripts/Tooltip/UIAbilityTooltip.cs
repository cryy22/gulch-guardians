using TMPro;
using UnityEngine;

namespace Tooltip
{
    public class UIAbilityTooltip : MonoBehaviour
    {
        private const string _sturdyTitle = "Sturdy";
        private const string _sturdyDescription = "\"Nice try, but I'm not going down that easily.\"";
        private const string _archerTitle = "Archer";
        private const string _archerDescription = "\"Attacks from 2nd position instead of 1st.\"";

        [SerializeField] private TMP_Text TitleText;
        [SerializeField] private TMP_Text DescriptionText;

        public void SetAbility(Tooltip.Ability ability)
        {
            if (ability == Tooltip.Ability.Sturdy)
            {
                TitleText.text = _sturdyTitle;
                DescriptionText.text = _sturdyDescription;
            }
            else if (ability == Tooltip.Ability.Archer)
            {
                TitleText.text = _archerTitle;
                DescriptionText.text = _archerDescription;
            }
        }

        public void Show() { gameObject.SetActive(true); }

        public void Hide() { gameObject.SetActive(false); }
    }
}
