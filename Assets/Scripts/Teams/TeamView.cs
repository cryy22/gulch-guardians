using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using Crysc.Presentation;
using GulchGuardians.Squads;
using UnityEngine;

namespace GulchGuardians.Teams
{
    [RequireComponent(typeof(Arrangement))]
    public class TeamView : MonoBehaviour, IArrangement<SquadView>
    {
        private readonly List<SquadView> _squads = new();
        private Arrangement _arrangement;

        [field: SerializeField] public Vector2 FrontSquadMaxSize { get; set; }
        [field: SerializeField] public Vector2 RemainingSquadsMaxSize { get; set; }

        private void Awake() { _arrangement = GetComponent<Arrangement>(); }

        private void Start() { _arrangement.IsInverted = IsInverted; }

        public void AddSquad(Squad squad, IEnumerable<Squad> squads)
        {
            squad.View.IsInverted = IsInverted;
            squad.View.Rearrange();

            SetElements(squads.Select(s => s.View));
            Rearrange();
        }

        private void ConfigureSquads()
        {
            if (_squads.Count == 0) return;

            SquadView frontSquad = _squads.First();
            foreach (SquadView squad in _squads)
            {
                squad.MaxSize = squad == frontSquad ? FrontSquadMaxSize : RemainingSquadsMaxSize;
                squad.ShowUnitUIs(squad == frontSquad);
                squad.UpdateProperties();
            }
        }

        // IArrangement
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

        public void SetElements(IEnumerable<SquadView> elements)
        {
            _squads.Clear();
            _squads.AddRange(elements);

            _arrangement.SetElements(elements);
        }

        public void Rearrange()
        {
            ConfigureSquads();
            foreach (SquadView squad in _squads) squad.Rearrange();

            _arrangement.Rearrange();
        }

        public IEnumerator AnimateRearrange(float duration = 0.25f)
        {
            ConfigureSquads();
            List<Coroutine> coroutines = (
                from squad in _squads
                select StartCoroutine(squad.AnimateRearrange(duration))
            ).ToList();
            coroutines.Add(StartCoroutine(_arrangement.AnimateRearrange(duration)));

            return CoroutineWaiter.RunConcurrently(coroutines.ToArray());
        }
    }
}
