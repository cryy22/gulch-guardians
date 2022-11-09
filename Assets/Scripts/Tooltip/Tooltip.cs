using TMPro;
using UnityEngine;

namespace Tooltip
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField] private GameObject Container;
        [SerializeField] private TMP_Text TitleText;
        [SerializeField] private TMP_Text Line1Text;
        [SerializeField] private TMP_Text Line2Text;
        [SerializeField] private TMP_Text Line3Text;

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

        public void Show() { Container.SetActive(true); }
        public void Hide() { Container.SetActive(false); }

        public void SetContent(string title, string line1, string line2, string line3)
        {
            TitleText.text = title;
            Line1Text.text = line1;
            Line2Text.text = line2;
            Line3Text.text = line3;
        }

        public void SetPosition(Vector2 position)
        {
            Container.transform.position = new Vector3(x: position.x, y: position.y, z: Container.transform.position.z);
        }
    }
}
