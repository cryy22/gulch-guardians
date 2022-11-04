using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class UIGameResultPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text ResultText;

        public void DisplayResult(bool isWin)
        {
            ResultText.text = isWin ? "You Win!" : "You Lose!";
            gameObject.SetActive(true);
        }
    }
}
