using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tooltip
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform Container;
        [SerializeField] private TMP_Text TitleText;
        [SerializeField] private TMP_Text Line1Text;
        [SerializeField] private TMP_Text Line2Text;
        [SerializeField] private TMP_Text Line3Text;

        private bool _showRequested;
        private bool _pointerIsOver;

        public static Tooltip Instance { get; private set; }

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Show() { Container.gameObject.SetActive(true); }

        public void Hide() { Container.gameObject.SetActive(false); }

        public void SetContent(string title, string line1, string line2, string line3)
        {
            TitleText.text = title;
            Line1Text.text = line1;
            Line2Text.text = line2;
            Line3Text.text = line3;
        }

        public void SetPosition(Vector2 position)
        {
            Container.position = new Vector3(x: position.x, y: position.y, z: Container.position.z);
        }

        public void OnPointerEnter(PointerEventData eventData) { Container.gameObject.SetActive(true); }

        public void OnPointerExit(PointerEventData eventData) { Container.gameObject.SetActive(false); }
    }
}
