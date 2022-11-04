using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GulchGuardians
{
    [RequireComponent(typeof(Image))]
    public class EffectOption : MonoBehaviour
    {
        [SerializeField] private TMP_Text EffectNameText;
        [SerializeField] private TMP_Text EffectDescriptionText;
        [SerializeField] private Button ChooseButton;

        private static readonly Color _selectedColor = new(r: 0.3882353f, g: 0.6431373f, b: 0.7764707f, a: 1f);

        private Image _image;
        private ModificationEffect _effect;

        public event EventHandler<EffectOptionClickedEventArgs> EffectOptionClicked;

        private void Awake() { _image = GetComponent<Image>(); }

        public void SetEffect(ModificationEffect effect)
        {
            _effect = effect;

            EffectNameText.text = effect.Name;
            EffectDescriptionText.text = effect.Description;
        }

        public void SetSelected(bool isSelected) { _image.color = isSelected ? _selectedColor : Color.black; }

        public void OnChooseButtonClicked()
        {
            EffectOptionClicked?.Invoke(sender: this, e: new EffectOptionClickedEventArgs(_effect));
        }

        public class EffectOptionClickedEventArgs : EventArgs
        {
            public EffectOptionClickedEventArgs(ModificationEffect effect) { Effect = effect; }
            public ModificationEffect Effect { get; }
        }
    }
}
