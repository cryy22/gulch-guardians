using System.Collections;
using TMPro;
using UnityEngine;

namespace GulchGuardians
{
    public class UIGameResultPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text ResultText;

        public IEnumerator DisplayResult(bool isWin)
        {
            ResultText.text = isWin ? "You Win!" : "You Lose!";
            gameObject.SetActive(true);

            yield return new WaitForSeconds(2f);
            gameObject.SetActive(false);
        }
    }
}
