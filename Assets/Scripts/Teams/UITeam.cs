using System.Collections;
using System.Collections.Generic;
using Crysc.UI;
using GulchGuardians.Squads;
using UnityEngine;

namespace GulchGuardians.Teams
{
    [RequireComponent(typeof(UIArrangement))]
    public class UITeam : MonoBehaviour
    {
        [SerializeField] private bool IsUnitOrderInverted;

        [SerializeField] private Vector2 FrontSquadMaxSize;
        [SerializeField] private Vector2 OtherSquadsMaxSize;

        private UIArrangement _arrangement;

        private void Awake() { _arrangement = GetComponent<UIArrangement>(); }

        public void AddSquad(Squad squad, IEnumerable<Squad> squads)
        {
            if (IsUnitOrderInverted) squad.UI.InvertArrangementOrder();
            StartCoroutine(UpdateArrangementElementsNextFrame(squads));
        }

        public IEnumerator AnimateUpdateElements(IEnumerable<Squad> squads)
        {
            yield return _arrangement.AnimateUpdateElements(squads);
        }

        private void UpdateElements(IEnumerable<Squad> squads) { _arrangement.UpdateElements(squads); }

        private IEnumerator UpdateArrangementElementsNextFrame(IEnumerable<Squad> squads)
        {
            yield return null;
            UpdateElements(squads);
        }
    }
}
