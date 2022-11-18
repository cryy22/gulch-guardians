using System;
using System.Collections.Generic;
using UnityEngine;

namespace GulchGuardians.ModificationEffects
{
    public class UIEffectOptionsDisplayer : MonoBehaviour
    {
        [SerializeField] private UIEffectOption EffectOptionPrefab;
        [SerializeField] private Transform EffectOptionParent;

        public event EventHandler EffectSelected;

        public ModificationEffect SelectedEffect { get; private set; }

        public void DisplayEffectOptions(IEnumerable<ModificationEffect> effects)
        {
            CleanUpEffectOptions();

            foreach (ModificationEffect effect in effects)
            {
                UIEffectOption effectOption = Instantiate(original: EffectOptionPrefab, parent: EffectOptionParent);
                effectOption.SetEffect(effect);
                effectOption.EffectOptionClicked += EffectOptionClickedEventHandler;
            }
        }

        public void CleanUpEffectOptions()
        {
            SelectedEffect = null;
            foreach (Transform child in EffectOptionParent) Destroy(child.gameObject);
        }

        private void EffectOptionClickedEventHandler(object sender, UIEffectOption.EffectOptionClickedEventArgs e)
        {
            SelectedEffect = e.Effect;
            foreach (UIEffectOption effectOption in EffectOptionParent.GetComponentsInChildren<UIEffectOption>())
                effectOption.SetSelected(effectOption == (UIEffectOption) sender);
            EffectSelected?.Invoke(sender: this, e: EventArgs.Empty);
        }
    }
}
