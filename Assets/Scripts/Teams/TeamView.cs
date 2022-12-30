using System.Collections;
using System.Collections.Generic;
using Crysc.Helpers;
using Crysc.Presentation;
using UnityEngine;

namespace GulchGuardians.Teams
{
    [RequireComponent(typeof(Arrangement))]
    public class TeamView : MonoBehaviour
    {
        private Team _team;
        private TeamArrangement _arrangement;

        [field: SerializeField] public Vector2 FrontSquadMaxSize { get; set; }
        [field: SerializeField] public Vector2 RemainingSquadsMaxSize { get; set; }
        [field: SerializeField] public Vector2 SquadSpacingRatio { get; set; }

        [field: SerializeField] public Vector2 CampSquadMaxSize { get; set; }
        [field: SerializeField] public Vector2 CampSquadSpacingRatio { get; set; }

        private void Awake()
        {
            _team = GetComponent<Team>();
            _arrangement = GetComponent<TeamArrangement>();
        }

        private void Start()
        {
            _arrangement.FrontSquadMaxSize = FrontSquadMaxSize;
            _arrangement.RemainingSquadsMaxSize = RemainingSquadsMaxSize;
            _arrangement.Rearrange();
        }

        public IEnumerator RearrangeForBattle(Transform campContainer)
        {
            Transform teamTransform = transform;
            teamTransform.SetParent(campContainer);

            _arrangement.FrontSquadMaxSize = FrontSquadMaxSize;
            _arrangement.IsCentered = false;
            _team.FrontSquad.View.PreferredSpacingRatio = SquadSpacingRatio;

            List<Coroutine> coroutines = new()
            {
                StartCoroutine(Mover.MoveTo(transform: teamTransform, end: Vector3.zero, duration: 0.5f)),
                StartCoroutine(AnimateUpdateArrangement(0.5f)),
            };

            return CoroutineWaiter.RunConcurrently(coroutines.ToArray());
        }

        public IEnumerator RearrangeForCamp(Transform campContainer)
        {
            Transform teamTransform = transform;
            teamTransform.SetParent(campContainer);

            _arrangement.FrontSquadMaxSize = CampSquadMaxSize;
            _arrangement.IsCentered = true;
            _team.FrontSquad.View.PreferredSpacingRatio = CampSquadSpacingRatio;

            List<Coroutine> coroutines = new()
            {
                StartCoroutine(Mover.MoveTo(transform: teamTransform, end: Vector3.zero, duration: 0.5f)),
                StartCoroutine(AnimateUpdateArrangement(0.5f)),
            };

            return CoroutineWaiter.RunConcurrently(coroutines.ToArray());
        }

        public void UpdateArrangement()
        {
            _arrangement.SetElements(_team.Squads);
            _arrangement.Rearrange();
        }

        public IEnumerator AnimateUpdateArrangement(float duration = 0.25f)
        {
            _arrangement.SetElements(_team.Squads);
            return _arrangement.AnimateRearrange(duration);
        }
    }
}
