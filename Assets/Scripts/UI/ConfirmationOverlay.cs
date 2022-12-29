using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GulchGuardians.UI
{
    public class ConfirmationOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject Container;
        [SerializeField] private Button ConfirmButton;
        [SerializeField] private Button CancelButton;

        private void Awake() { Container.SetActive(false); }

        public void Show(Action onConfirm, Action onCancel)
        {
            Container.SetActive(true);
            ConfirmButton.onClick.AddListener(SelectionResponse(onConfirm));
            CancelButton.onClick.AddListener(SelectionResponse(onCancel));
        }

        private UnityAction SelectionResponse(Action callback)
        {
            return () =>
            {
                callback?.Invoke();
                Container.SetActive(false);
            };
        }
    }
}
