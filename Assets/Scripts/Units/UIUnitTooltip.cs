using System.Collections.Generic;
using System.Linq;
using Crysc.UI;
using GulchGuardians.Abilities;
using TMPro;
using UnityEngine;

namespace GulchGuardians.Units
{
    public class UIUnitTooltip : UITooltip<Unit>
    {
        [SerializeField] private TMP_Text TitleText;
        [SerializeField] private TMP_Text Line1Text;
        [SerializeField] private TMP_Text Line2Text;
        [SerializeField] private TMP_Text Line3Text;

        [SerializeField] private UIAbilityTextItem AbilityTextItem1;
        [SerializeField] private UIAbilityTextItem AbilityTextItem2;
        [SerializeField] private UIAbilityTextItem AbilityTextItem3;

        private readonly List<AbilityType> _abilities = new();

        private List<UIAbilityTextItem> TooltipItems => new()
        {
            AbilityTextItem1,
            AbilityTextItem2,
            AbilityTextItem3,
        };

        protected override void ShowTooltip(Unit unit)
        {
            if (unit.TooltipEnabled == false) return;

            base.ShowTooltip(unit);
            SetContent(unit);
            SetAbilities(unit);
        }

        private static string TwoDigitNumber(int number) { return number.ToString("00"); }

        private void SetContent(Unit unit)
        {
            TitleText.text = unit.FirstName;
            Line1Text.text = $"attack {TwoDigitNumber(unit.Attack)}";
            Line2Text.text = $"health {TwoDigitNumber(unit.Health)}";
            Line3Text.text = $"maxhlth {TwoDigitNumber(unit.MaxHealth)}";
        }

        private void SetAbilities(Unit unit)
        {
            _abilities.Clear();
            _abilities.AddRange(unit.ActiveAbilities);

            foreach ((UIAbilityTextItem item, int i) in TooltipItems.Select((el, i) => (el, i)))
                item.SetAbility(_abilities.ElementAtOrDefault(i));
        }
    }
}
