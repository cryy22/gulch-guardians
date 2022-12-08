using GulchGuardians.Constants;
using GulchGuardians.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GulchGuardians.SceneControls
{
    [RequireComponent(typeof(Button))]
    public class UIRestartButton : MonoBehaviour
    {
        [SerializeField] private UIConfirmationOverlay ConfirmationOverlay;

        private Button _button;

        private void Awake() { _button = GetComponent<Button>(); }
        private void OnEnable() { _button.onClick.AddListener(OnButtonClicked); }
        private void OnDisable() { _button.onClick.RemoveListener(OnButtonClicked); }

        private void OnButtonClicked() { ConfirmationOverlay.Show(onConfirm: RestartAction, onCancel: CancelAction); }
        private void RestartAction() { SceneManager.LoadScene(Scenes.MainIndex); }
        private void CancelAction() { gameObject.SetActive(true); }
    }
}
