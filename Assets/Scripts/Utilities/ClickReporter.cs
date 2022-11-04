using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GulchGuardians
{
    [RequireComponent(typeof(Collider2D))]
    public class ClickReporter : MonoBehaviour
    {
        public Object Sender;
        public event EventHandler Clicked;

        private void OnMouseDown() { Clicked?.Invoke(sender: Sender, e: EventArgs.Empty); }
    }
}
