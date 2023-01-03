using System;
using System.Collections.Generic;
using UnityEngine;

namespace GulchGuardians.Promotions
{
    public class PromotionChooser : MonoBehaviour
    {
        [SerializeField] private List<PromotionOption> PromotionOptions;

        public event EventHandler<PromotionSelectedEventArgs> Selected;

        private void OnEnable()
        {
            foreach (PromotionOption option in PromotionOptions) option.Selected += OptionClickedEventHandler;
        }

        private void OnDisable()
        {
            foreach (PromotionOption option in PromotionOptions) option.Selected -= OptionClickedEventHandler;
        }

        private void OptionClickedEventHandler(object sender, PromotionSelectedEventArgs e)
        {
            Selected?.Invoke(sender: this, e: e);
        }
    }
}
