using System;
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

        [SerializeField] private UIAbilityTooltipItem AbilityTooltipItem1;
        [SerializeField] private UIAbilityTooltipItem AbilityTooltipItem2;
        [SerializeField] private UIAbilityTooltipItem AbilityTooltipItem3;

        [SerializeField] private UIAbilityTooltip AbilityTooltip;

        private readonly List<AbilityType> _abilities = new();

        private Camera _camera;

        private List<UIAbilityTooltipItem> TooltipItems => new()
        {
            AbilityTooltipItem1,
            AbilityTooltipItem2,
            AbilityTooltipItem3,
        };

        private void Awake() { _camera = Camera.main; }

        protected override void OnEnable()
        {
            base.OnEnable();

            foreach (UIAbilityTooltipItem tooltipItem in TooltipItems)
            {
                tooltipItem.PointerEntered += AbilityTooltipItemPointerEnteredEventHandler;
                tooltipItem.PointerExited += AbilityTooltipItemPointerExitedEventHandler;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (UIAbilityTooltipItem tooltipItem in TooltipItems)
            {
                tooltipItem.PointerEntered -= AbilityTooltipItemPointerEnteredEventHandler;
                tooltipItem.PointerExited -= AbilityTooltipItemPointerExitedEventHandler;
            }
        }

        protected override void ShowTooltip(Unit unit)
        {
            if (unit.TooltipEnabled == false) return;

            base.ShowTooltip(unit);

            SetContent(unit);
            SetAbilities(unit);
            SetPosition(unit);
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

            foreach ((UIAbilityTooltipItem item, int i) in TooltipItems.Select((el, i) => (el, i)))
                if (i >= _abilities.Count)
                    item.SetTitle("----");
                else
                    item.SetTitle(_abilities[i].Name);
        }

        private void SetPosition(Unit unit)
        {
            var worldPoint = new Vector3(
                x: unit.Bounds.center.x,
                y: unit.Bounds.max.y - 0.25f,
                z: 0
            );
            Vector3 screenPoint = _camera.WorldToScreenPoint(worldPoint);
            Container.transform.position = new Vector3(
                x: screenPoint.x,
                y: screenPoint.y,
                z: Container.transform.position.z
            );
        }

        private void AbilityTooltipItemPointerEnteredEventHandler(object sender, EventArgs e)
        {
            int index = TooltipItems.IndexOf((UIAbilityTooltipItem) sender);
            if (index >= _abilities.Count) return;

            AbilityTooltip.SetAbility(_abilities[index]);
            AbilityTooltip.Show();
        }

        private void AbilityTooltipItemPointerExitedEventHandler(object sender, EventArgs e) { AbilityTooltip.Hide(); }
    }
}
