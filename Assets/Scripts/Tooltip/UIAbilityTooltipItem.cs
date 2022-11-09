using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Abilities
{
    public class UIAbilityTooltipItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text Name;

        public event EventHandler PointerEntered;
        public event EventHandler PointerExited;

        public void SetTitle(string title) { Name.text = title; }

        public void OnPointerEnter(PointerEventData _) { PointerEntered?.Invoke(sender: this, e: EventArgs.Empty); }

        public void OnPointerExit(PointerEventData _) { PointerExited?.Invoke(sender: this, e: EventArgs.Empty); }
    }
}
