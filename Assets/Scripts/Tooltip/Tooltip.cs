using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tooltip
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform Container;
        [SerializeField] private TMP_Text TitleText;
        [SerializeField] private TMP_Text Line1Text;
        [SerializeField] private TMP_Text Line2Text;
        [SerializeField] private TMP_Text Line3Text;

        [SerializeField] private UIAbilityTooltipItem AbilityTooltipItem1;
        [SerializeField] private UIAbilityTooltipItem AbilityTooltipItem2;
        [SerializeField] private UIAbilityTooltipItem AbilityTooltipItem3;

        [SerializeField] private UIAbilityTooltip AbilityTooltip;

        private readonly List<AbilityType> _abilities = new();

        private bool _showRequested;
        private bool _pointerIsOver;

        public static Tooltip Instance { get; private set; }

        private List<UIAbilityTooltipItem> TooltipItems => new()
        {
            AbilityTooltipItem1,
            AbilityTooltipItem2,
            AbilityTooltipItem3,
        };

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            foreach (UIAbilityTooltipItem tooltipItem in TooltipItems)
            {
                tooltipItem.PointerEntered += AbilityTooltipItemPointerEnteredEventHandler;
                tooltipItem.PointerExited += AbilityTooltipItemPointerExitedEventHandler;
            }
        }

        private void OnDisable()
        {
            foreach (UIAbilityTooltipItem tooltipItem in TooltipItems)
            {
                tooltipItem.PointerEntered -= AbilityTooltipItemPointerEnteredEventHandler;
                tooltipItem.PointerExited -= AbilityTooltipItemPointerExitedEventHandler;
            }
        }

        public void Show() { Container.gameObject.SetActive(true); }

        public void Hide() { Container.gameObject.SetActive(false); }

        public void SetContent(string title, string line1, string line2, string line3)
        {
            TitleText.text = title;
            Line1Text.text = line1;
            Line2Text.text = line2;
            Line3Text.text = line3;
        }

        public void SetAbilities(IEnumerable<AbilityType> abilities)
        {
            _abilities.Clear();
            _abilities.AddRange(abilities);

            foreach ((UIAbilityTooltipItem item, int i) in TooltipItems.Select((el, i) => (el, i)))
                if (i >= _abilities.Count)
                    item.SetTitle("----");
                else
                    item.SetTitle(_abilities[i].Name);
        }

        public void SetPosition(Vector2 position)
        {
            Container.position = new Vector3(x: position.x, y: position.y, z: Container.position.z);
        }

        private void AbilityTooltipItemPointerEnteredEventHandler(object sender, EventArgs e)
        {
            int index = TooltipItems.IndexOf((UIAbilityTooltipItem) sender);
            if (index >= _abilities.Count) return;

            AbilityTooltip.SetAbility(_abilities[index]);
            AbilityTooltip.Show();
        }

        private void AbilityTooltipItemPointerExitedEventHandler(object sender, EventArgs e) { AbilityTooltip.Hide(); }

        public void OnPointerEnter(PointerEventData eventData) { Container.gameObject.SetActive(true); }

        public void OnPointerExit(PointerEventData eventData) { Container.gameObject.SetActive(false); }
    }
}
