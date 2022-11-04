using UnityEngine;

namespace InfiniteSAPPrototype
{
    [RequireComponent(typeof(Collider2D))]
    public class ClickReporter : MonoBehaviour
    {
        public delegate void OnReporterClicked(ClickReporter reporter);
        public event OnReporterClicked OnReporterClickedEvent;

        private void OnMouseDown() { OnReporterClickedEvent?.Invoke(this); }
    }
}
