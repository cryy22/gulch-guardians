using System;
using GulchGuardians.Abilities;
using UnityEngine;

namespace GulchGuardians.Promotions
{
    public class PromotionOption : MonoBehaviour
    {
        [SerializeField] private AbilityType Ability;

        public event EventHandler<PromotionSelectedEventArgs> Selected;

        private void OnMouseUpAsButton() { Selected?.Invoke(sender: this, e: new PromotionSelectedEventArgs(Ability)); }
    }
}
