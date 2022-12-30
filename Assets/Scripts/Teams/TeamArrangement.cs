using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using Crysc.Presentation;
using GulchGuardians.Squads;
using UnityEngine;

namespace GulchGuardians.Teams
{
    [RequireComponent(typeof(IArrangement<Squad>))]
    public class TeamArrangement : MonoBehaviour, IArrangement<Squad>
    {
        private readonly List<SquadView> _squads = new();
        private Arrangement _arrangement;

        public Vector2 FrontSquadMaxSize { get; set; }
        public Vector2 RemainingSquadsMaxSize { get; set; }

        private void Awake() { _arrangement = GetComponent<Arrangement>(); }

        private void Start() { _arrangement.IsInverted = IsInverted; }

        private void ConfigureSquads()
        {
            if (_squads.Count == 0) return;

            SquadView frontSquad = _squads.First();
            foreach (SquadView squad in _squads)
            {
                squad.Arrangement.IsInverted = IsInverted;
                squad.Arrangement.MaxSize = squad == frontSquad ? FrontSquadMaxSize : RemainingSquadsMaxSize;

                squad.ShowUnitUIs(squad == frontSquad);
                squad.Arrangement.UpdateProperties();
            }
        }

        // IArrangement<Squad>
        public bool IsCentered
        {
            get => _arrangement.IsCentered;
            set => _arrangement.IsCentered = value;
        }

        public bool IsInverted
        {
            get => _arrangement.IsInverted;
            set => _arrangement.IsInverted = value;
        }

        public Vector2 MaxSize
        {
            get => _arrangement.MaxSize;
            set => _arrangement.MaxSize = value;
        }

        public Vector2 PreferredSpacingRatio
        {
            get => _arrangement.PreferredSpacingRatio;
            set => _arrangement.PreferredSpacingRatio = value;
        }

        public void SetElements(IEnumerable<Squad> elements)
        {
            List<SquadView> squadViews = elements.Select(s => s.View).ToList();
            _squads.Clear();
            _squads.AddRange(squadViews);

            _arrangement.SetElements(squadViews);
        }

        public void Rearrange()
        {
            ConfigureSquads();
            foreach (SquadView squad in _squads) squad.UpdateArrangement();

            _arrangement.Rearrange();
        }

        public IEnumerator AnimateRearrange(float duration = 0.25f)
        {
            ConfigureSquads();
            List<Coroutine> coroutines = (
                from squad in _squads
                select StartCoroutine(squad.AnimateUpdateArrangement(duration))
            ).ToList();
            coroutines.Add(StartCoroutine(_arrangement.AnimateRearrange(duration)));

            return CoroutineWaiter.RunConcurrently(coroutines.ToArray());
        }
    }
}
