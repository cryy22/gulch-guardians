using System;
using GulchGuardians.Abilities;

namespace GulchGuardians.Promotions
{
    public class PromotionSelectedEventArgs : EventArgs
    {
        public PromotionSelectedEventArgs(AbilityType ability) { Ability = ability; }
        public AbilityType Ability { get; }
    }
}
