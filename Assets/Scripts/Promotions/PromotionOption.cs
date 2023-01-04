using System;
using GulchGuardians.Classes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GulchGuardians.Promotions
{
    [RequireComponent(typeof(Button))]
    public class PromotionOption : MonoBehaviour
    {
        [SerializeField] private ClassType Class;

        private Button _button;
        private TMP_Text _text;

        public event EventHandler<PromotionSelectedEventArgs> Selected;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _text = _button.GetComponentInChildren<TMP_Text>();
        }

        private void Start()
        {
            _text.text = Class.Name;
            _button.onClick.AddListener(OnClicked);
        }

        private void OnClicked() { Selected?.Invoke(sender: this, e: new PromotionSelectedEventArgs(Class)); }
    }
}
