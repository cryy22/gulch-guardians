using UnityEngine;
using UnityEngine.UI;

namespace GulchGuardians.UI
{
    [ExecuteAlways]
    public class UIDynamicScrollRect : ScrollRect
    {
        [SerializeField] private float MaxHeight = 100;

        private RectTransform _rectTransform;

        protected override void Awake()
        {
            base.Awake();
            _rectTransform = GetComponent<RectTransform>();
        }

        protected override void LateUpdate()
        {
            if (content != null)
                _rectTransform.sizeDelta = new Vector2(
                    x: _rectTransform.sizeDelta.x,
                    y: Mathf.Clamp(value: content.sizeDelta.y + 1, min: 0, max: MaxHeight)
                );
            base.LateUpdate();
        }
    }
}
