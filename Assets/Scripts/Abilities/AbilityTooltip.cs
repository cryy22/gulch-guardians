using Crysc.UI;
using TMPro;
using UnityEngine;

namespace GulchGuardians.Abilities
{
    public class AbilityTooltip : Tooltip<AbilityType>
    {
        [SerializeField] private TMP_Text TitleText;
        [SerializeField] private TMP_Text DescriptionText;

        protected override void ShowTooltip(AbilityType target)
        {
            if (target == null) return;

            base.ShowTooltip(target);
            SetAbility(target);
        }

        private void SetAbility(AbilityType ability)
        {
            TitleText.text = ability.Name;
            DescriptionText.text = ability.Description;
        }
    }
}
