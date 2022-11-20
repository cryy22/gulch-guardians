using System;
using Crysc.Registries;
using UnityEngine;

namespace GulchGuardians.Units
{
    [CreateAssetMenu(fileName = "UnitRegistry", menuName = "Registries/Unit")]
    public class UnitRegistry : Registry<Unit>
    {
        public event EventHandler Destroying;

        protected override void SubscribeToEvents(Unit unit) { unit.Destroying += DestroyingEventHandler; }
        protected override void UnsubscribeFromEvents(Unit unit) { unit.Destroying -= DestroyingEventHandler; }
        private void DestroyingEventHandler(object sender, EventArgs e) { Destroying?.Invoke(sender: sender, e: e); }
    }
}
