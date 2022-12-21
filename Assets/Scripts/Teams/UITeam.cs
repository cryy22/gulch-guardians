using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using Crysc.UI;
using GulchGuardians.Squads;
using UnityEngine;

namespace GulchGuardians.Teams
{
    [RequireComponent(typeof(UIArrangement))]
    public class UITeam : MonoBehaviour
    {
        [SerializeField] private Vector2 FrontSquadMaxSize;
        [SerializeField] private Vector2 RemainingSquadsMaxSize;
        [SerializeField] private bool IsUnitOrderInverted;

        private UIArrangement _arrangement;

        private void Awake() { _arrangement = GetComponent<UIArrangement>(); }

        public void AddSquad(Squad squad, IEnumerable<Squad> squads)
        {
            if (IsUnitOrderInverted) squad.UI.InvertArrangementOrder();
            UpdateElements(squads);
        }

        public IEnumerator AnimateUpdateElements(IEnumerable<Squad> squads)
        {
            squads = squads.ToList();
            if (squads.Count() == 0) yield break;
            Squad frontSquad = squads.First();

            List<Coroutine> coroutines = (
                from squad in squads
                let maxSize = squad == frontSquad ? FrontSquadMaxSize : RemainingSquadsMaxSize
                select StartCoroutine(squad.UI.AnimateUpdateMaxSize(maxSize))
            ).ToList();
            coroutines.Add(StartCoroutine(_arrangement.AnimateUpdateElements(squads.Select(s => s.UI))));

            yield return CoroutineWaiter.RunConcurrently(coroutines.ToArray());
        }

        private void UpdateElements(IEnumerable<Squad> squads)
        {
            squads = squads.ToList();
            if (squads.Count() == 0) return;
            Squad frontSquad = squads.First();

            foreach (Squad squad in squads)
                squad.UI.UpdateMaxSize(squad == frontSquad ? FrontSquadMaxSize : RemainingSquadsMaxSize);

            _arrangement.UpdateElements(squads.Select(s => s.UI));
        }
    }
}
