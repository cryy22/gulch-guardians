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
        [SerializeField] public Vector2 FrontSquadMaxSize;
        [SerializeField] private Vector2 RemainingSquadsMaxSize;
        [SerializeField] private bool IsOrderInverted;

        private UIArrangement _arrangement;

        private void Awake() { _arrangement = GetComponent<UIArrangement>(); }

        public void AddSquad(Squad squad, IEnumerable<Squad> squads)
        {
            squad.UI.IsInverted = IsOrderInverted;
            squad.UI.Rearrange();

            UpdateElements(squads);
        }

        public IEnumerator AnimateUpdateElements(IEnumerable<Squad> squads)
        {
            squads = squads.ToList();
            if (squads.Count() == 0) yield break;
            Squad frontSquad = squads.First();

            // List<Coroutine> coroutines = (
            //     from squad in squads
            //     let maxSize = squad == frontSquad ? FrontSquadMaxSize : RemainingSquadsMaxSize
            //     select StartCoroutine(squad.UI.AnimateUpdateMaxSize(maxSize))
            // ).ToList();

            foreach (Squad squad in squads)
                squad.UI.MaxSize = squad == frontSquad ? FrontSquadMaxSize : RemainingSquadsMaxSize;

            List<Coroutine> coroutines = (
                from squad in squads
                select StartCoroutine(squad.UI.AnimateRearrange())
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
            {
                squad.UI.MaxSize = squad == frontSquad ? FrontSquadMaxSize : RemainingSquadsMaxSize;
                squad.UI.Rearrange();
            }

            _arrangement.UpdateElements(squads.Select(s => s.UI));
        }
    }
}
