using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tooltip
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField] private TMP_Text NameText;
        [SerializeField] private TMP_Text AttackText;
        [SerializeField] private TMP_Text HealthText;
        [SerializeField] private TMP_Text MaxHealthText;

        private Camera _camera;

        public Tooltip Instance { get; private set; }

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

        public void OnPointerMove(InputAction.CallbackContext context)
        {
            Vector3 mousePosition = _camera.ScreenToWorldPoint(context.ReadValue<Vector2>());
            transform.position = new Vector3(
                x: mousePosition.x,
                y: mousePosition.y + 0.1f,
                z: transform.position.z
            );
        }
    }
}
