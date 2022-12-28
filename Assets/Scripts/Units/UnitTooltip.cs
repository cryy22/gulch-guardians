using System;
using System.Collections.Generic;
using System.Linq;
using Crysc.UI;
using GulchGuardians.Abilities;
using TMPro;
using UnityEngine;

namespace GulchGuardians.Units
{
    public class UnitTooltip : Tooltip<Unit>
    {
        [SerializeField] private TMP_Text TitleText;
        [SerializeField] private TMP_Text Line1Text;
        [SerializeField] private TMP_Text Line2Text;
        [SerializeField] private TMP_Text Line3Text;

        [SerializeField] private Transform AbilityTextItemParent;
        [SerializeField] private UIAbilityTextItem AbilityTextItemPrefab;

        private readonly Dictionary<AbilityType, UIAbilityTextItem> _abilitiesItems = new();

        protected override bool ShouldShowTooltip(Unit target) { return target.View.ShowTooltip; }

        protected override void UpdateTarget(Unit target, Unit previousTarget)
        {
            base.UpdateTarget(target: target, previousTarget: previousTarget);

            SetContent(target);
            SetAbilities(target);

            if (previousTarget != null) previousTarget.Changed -= ChangedEventHandler;
            target.Changed += ChangedEventHandler;
        }

        private static string TwoDigitNumber(int number) { return number.ToString("00"); }

        private void ChangedEventHandler(object sender, EventArgs _)
        {
            var unit = (Unit) sender;

            SetContent(unit);
            SetAbilities(unit);
        }

        private void SetContent(Unit unit)
        {
            TitleText.text = unit.FirstName;
            Line1Text.text = $"attack {TwoDigitNumber(unit.Attack)}";
            Line2Text.text = $"health {TwoDigitNumber(unit.Health)}";
            Line3Text.text = $"maxhlth {TwoDigitNumber(unit.MaxHealth)}";
        }

        private void SetAbilities(Unit unit)
        {
            List<AbilityType> abilities = unit.Abilities.ToList();

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
