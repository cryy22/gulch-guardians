using System.Collections;
using System.Collections.Generic;
using Crysc.Coordination;
using Crysc.Helpers;
using Crysc.UI;
using GulchGuardians.Teams;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GulchGuardians.Coordination
{
    public class CampCoordinator : Coordinator
    {
        [SerializeField] private Button AdvanceButton;
        [SerializeField] private Transform PlayerTeamContainer;
        [SerializeField] private Team PlayerTeam;
        [SerializeField] private UIParallaxBackground Background;
        [SerializeField] private GameState State;

        private static readonly Vector2 _squadMaxSize = Vector2.zero;
        private static readonly Vector2 _squadSpacingRatio = new(x: 0.5f, y: 0);

        private TMP_Text _advanceButtonText;

        private bool IsCorrectPhase => State.NightPhase == NightPhase.Camp;

        protected override void Awake()
        {
            base.Awake();
            _advanceButtonText = AdvanceButton.GetComponentInChildren<TMP_Text>();
        }

        private void Start() { AdvanceButton.onClick.AddListener(OnAdvanceButtonClicked); }

        public override void BeginCoordination()
        {
            base.BeginCoordination();

            _advanceButtonText.text = "battle";
            AdvanceButton.interactable = true;
            Background.SetCurtain(true);

            StartCoroutine(OnboardPlayerTeam());
        }

        public override void EndCoordination()
        {
            Background.SetCurtain(false);

            base.EndCoordination();
        }

        private void OnAdvanceButtonClicked()
        {
            if (!IsCorrectPhase) return;
            EndCoordination();
        }

        private IEnumerator OnboardPlayerTeam()
        {
            PlayerTeam.transform.SetParent(PlayerTeamContainer);

            PlayerTeam.View.FrontSquadMaxSize = _squadMaxSize;
            PlayerTeam.View.IsCentered = true;
            PlayerTeam.FrontSquad.View.PreferredSpacingRatio = _squadSpacingRatio;

            List<Coroutine> coroutines = new()
            {
                StartCoroutine(Mover.MoveTo(transform: PlayerTeam.transform, end: Vector3.zero, duration: 0.5f)),
                StartCoroutine(PlayerTeam.View.AnimateRearrange(0.5f)),
            };

            yield return CoroutineWaiter.RunConcurrently(coroutines.ToArray());
        }
    }
}
