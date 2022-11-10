using System;
using System.Collections.Generic;
using UnityEngine;

namespace GulchGuardians
{
    public class EffectOptionsDisplayer : MonoBehaviour
    {
        [SerializeField] private EffectOption EffectOptionPrefab;
        [SerializeField] private Transform EffectOptionParent;

        public event EventHandler EffectSelected;

        public ModificationEffect SelectedEffect { get; private set; }

        public void DisplayEffectOptions(IEnumerable<ModificationEffect> effects)
        {
            CleanUpEffectOptions();

            foreach (ModificationEffect effect in effects)
            {
                EffectOption effectOption = Instantiate(original: EffectOptionPrefab, parent: EffectOptionParent);
                effectOption.SetEffect(effect);
                effectOption.EffectOptionClicked += EffectOptionClickedEventHandler;
            }
        }

        public void CleanUpEffectOptions()
        {
            SelectedEffect = null;
            foreach (Transform child in EffectOptionParent) Destroy(child.gameObject);
        }

        private void EffectOptionClickedEventHandler(object sender, EffectOption.EffectOptionClickedEventArgs e)
        {
            SelectedEffect = e.Effect;
            foreach (EffectOption effectOption in EffectOptionParent.GetComponentsInChildren<EffectOption>())
                effectOption.SetSelected(effectOption == (EffectOption) sender);
            EffectSelected?.Invoke(sender: this, e: EventArgs.Empty);
        }
    }
}
