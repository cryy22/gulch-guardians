using GulchGuardians;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tooltip
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Unit))]
    public class UnitTooltipDisplayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Unit _unit;

        public void Awake() { _unit = GetComponent<Unit>(); }

        private static string TwoDigitNumber(int number) { return number.ToString("00"); }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_unit.TooltipEnabled == false) return;

            Tooltip.Instance.SetContent(
                title: _unit.FirstName,
                line1: $"attack {TwoDigitNumber(_unit.Attack)}",
                line2: $"health {TwoDigitNumber(_unit.Health)}",
                line3: $"maxhlth {TwoDigitNumber(_unit.MaxHealth)}"
            );
            Tooltip.Instance.Show();
        }

        public void OnPointerExit(PointerEventData eventData) { Tooltip.Instance.Hide(); }
    }
}
