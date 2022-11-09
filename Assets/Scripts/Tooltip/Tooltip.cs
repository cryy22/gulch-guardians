using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tooltip
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField] private GameObject Container;
        [SerializeField] private TMP_Text TitleText;
        [SerializeField] private TMP_Text Line1Text;
        [SerializeField] private TMP_Text Line2Text;
        [SerializeField] private TMP_Text Line3Text;

        private Camera _camera;

        public static Tooltip Instance { get; private set; }

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _camera = Camera.main;
        }

        public void Show() { Container.SetActive(true); }
        public void Hide() { Container.SetActive(false); }

        public void SetContent(string title, string line1, string line2, string line3)
        {
            TitleText.text = title;
            Line1Text.text = line1;
            Line2Text.text = line2;
            Line3Text.text = line3;
        }

        public void OnPointerMove(InputAction.CallbackContext context)
        {
            if (context.performed == false) return;

            var mousePosition = context.ReadValue<Vector2>();
            Container.transform.position = new Vector3(
                x: mousePosition.x,
                y: mousePosition.y + 40f,
                z: transform.position.z
            );
        }
    }
}
