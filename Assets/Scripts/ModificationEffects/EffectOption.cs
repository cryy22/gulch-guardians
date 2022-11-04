using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GulchGuardians
{
    public class EffectOption : MonoBehaviour
    {
        [SerializeField] private TMP_Text EffectNameText;
        [SerializeField] private RectTransform EffectDescriptionPanel;
        [SerializeField] private TMP_Text EffectDescriptionText;

        private static readonly Color _selectedColor = new(r: 0.3882353f, g: 0.6431373f, b: 0.7764707f, a: 1f);

        private ModificationEffect _effect;

        public event EventHandler<EffectOptionClickedEventArgs> EffectOptionClicked;

        public void SetEffect(ModificationEffect effect)
        {
            _effect = effect;

            EffectNameText.text = effect.Name;

            GameObject previewGameObject = effect.GetPreviewGameObject();

            if (previewGameObject == null)
            {
                EffectDescriptionText.text = effect.Description;
                EffectDescriptionPanel.gameObject.SetActive(true);
                return;
            }

            EffectDescriptionPanel.gameObject.SetActive(false);

            var unit = previewGameObject.GetComponent<Unit>();
            if (unit != null) StartCoroutine(UpdateUnitPosition(unit));
            else throw new Exception("Unhandled PreviewGameObject type");
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

        private IEnumerator UpdateUnitPosition(Unit unit)
        {
            yield return new WaitForEndOfFrame();
            unit.transform.position = new Vector3(
                x: EffectDescriptionPanel.position.x,
                y: EffectDescriptionPanel.position.y - 0.85f,
                z: unit.transform.position.z
            );
            unit.transform.localScale = Vector3.one * 0.85f;
        }

        public class EffectOptionClickedEventArgs : EventArgs
        {
            public EffectOptionClickedEventArgs(ModificationEffect effect) { Effect = effect; }
            public ModificationEffect Effect { get; }
        }
    }
}
