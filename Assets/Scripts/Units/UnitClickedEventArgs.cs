using System;

namespace GulchGuardians.Units
{
    public class UnitClickedEventArgs : EventArgs
    {
        public UnitClickedEventArgs(Unit unit) { Unit = unit; }
        public Unit Unit { get; }
    }
}
