using GulchGuardians.Constants;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GulchGuardians.SceneControls
{
    [RequireComponent(typeof(Button))]
    public class UIResetButton : MonoBehaviour
    {
        private Button _button;

        private void Awake() { _button = GetComponent<Button>(); }
        private void OnEnable() { _button.onClick.AddListener(OnButtonClicked); }
        private void OnDisable() { _button.onClick.RemoveListener(OnButtonClicked); }

        private static void OnButtonClicked() { SceneManager.LoadScene(Scenes.MainIndex); }
    }
}
