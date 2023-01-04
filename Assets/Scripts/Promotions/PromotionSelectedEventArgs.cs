using System;
using GulchGuardians.Classes;

namespace GulchGuardians.Promotions
{
    public class PromotionSelectedEventArgs : EventArgs
    {
        public PromotionSelectedEventArgs(ClassType @class) { Class = @class; }
        public ClassType Class { get; }
    }
}
