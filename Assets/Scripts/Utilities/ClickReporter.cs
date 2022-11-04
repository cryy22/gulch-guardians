using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace GulchGuardians
{
    [RequireComponent(typeof(Collider2D))]
    public class ClickReporter : MonoBehaviour, IPointerClickHandler
    {
        public Object Sender;
        public event EventHandler Clicked;

        public void OnPointerClick(PointerEventData eventData) { Clicked?.Invoke(sender: Sender, e: EventArgs.Empty); }
    }
}
