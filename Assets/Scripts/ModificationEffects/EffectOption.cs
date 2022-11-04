using System;
using TMPro;
using UnityEngine;

namespace GulchGuardians
{
    public class EffectOption : MonoBehaviour
    {
        [SerializeField] private TMP_Text EffectNameText;
        [SerializeField] private TMP_Text EffectDescriptionText;

        private static readonly Color _selectedColor = new(r: 0.3882353f, g: 0.6431373f, b: 0.7764707f, a: 1f);

        private ModificationEffect _effect;

        public event EventHandler<EffectOptionClickedEventArgs> EffectOptionClicked;

        public void SetEffect(ModificationEffect effect)
        {
            _effect = effect;

            EffectNameText.text = effect.Name;
            EffectDescriptionText.text = effect.Description;
        }

        public void SetSelected(bool isSelected)
        {
            EffectNameText.color = isSelected ? _selectedColor : Color.white;
            EffectDescriptionText.color = isSelected ? _selectedColor : Color.white;
        }

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
