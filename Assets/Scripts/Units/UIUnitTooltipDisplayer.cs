using UnityEngine;
using UnityEngine.EventSystems;

namespace GulchGuardians.Units
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Unit))]
    public class UIUnitTooltipDisplayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Unit _unit;
        private Bounds _bounds;
        private Camera _camera;

        public void Awake()
        {
            _unit = GetComponent<Unit>();
            _bounds = GetComponent<Collider2D>().bounds;
            _camera = Camera.main;
        }

        private static string TwoDigitNumber(int number) { return number.ToString("00"); }

        private Vector2 GetTooltipPosition()
        {
            Vector3 worldPoint = transform.TransformPoint(
                new Vector3(
                    x: _bounds.center.x,
                    y: _bounds.max.y - 0.25f,
                    z: 0
                )
            );

            return _camera.WorldToScreenPoint(worldPoint);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_unit.TooltipEnabled == false) return;

            UIUnitTooltip.Instance.SetContent(
                title: _unit.FirstName,
                line1: $"attack {TwoDigitNumber(_unit.Attack)}",
                line2: $"health {TwoDigitNumber(_unit.Health)}",
                line3: $"maxhlth {TwoDigitNumber(_unit.MaxHealth)}"
            );
            UIUnitTooltip.Instance.SetAbilities(_unit.ActiveAbilities);
            UIUnitTooltip.Instance.SetPosition(GetTooltipPosition());
            UIUnitTooltip.Instance.Show();
        }

        public void OnPointerExit(PointerEventData eventData) { UIUnitTooltip.Instance.Hide(); }
    }
}
