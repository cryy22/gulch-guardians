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

        [SerializeField] private Transform AbilityTextItemParent;
        [SerializeField] private UIAbilityTextItem AbilityTextItemPrefab;

        private readonly Dictionary<AbilityType, UIAbilityTextItem> _abilitiesItems = new();

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
            List<AbilityType> abilities = unit.ActiveAbilities.ToList();

            foreach (AbilityType ability in abilities)
            {
                if (_abilitiesItems.ContainsKey(ability)) continue;

                UIAbilityTextItem item = Instantiate(original: AbilityTextItemPrefab, parent: AbilityTextItemParent);
                item.SetAbility(ability);
                _abilitiesItems.Add(key: ability, value: item);
            }

            foreach (AbilityType ability in _abilitiesItems.Keys.Except(abilities).ToList())
            {
                Destroy(_abilitiesItems[ability].gameObject);
                _abilitiesItems.Remove(ability);
            }
        }
    }
}
