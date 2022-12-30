using Crysc.Coordination;
using Crysc.Presentation;
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
        [SerializeField] private ParallaxBackground Background;
        [SerializeField] private GameState State;

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

            StartCoroutine(PlayerTeam.View.RearrangeForCamp(PlayerTeamContainer));
            PlayerTeam.FrontSquad.Reorderer.BeginReordering();
        }

        public override void EndCoordination()
        {
            PlayerTeam.FrontSquad.Reorderer.EndReordering();
            Background.SetCurtain(false);

            base.EndCoordination();
        }

        private void OnAdvanceButtonClicked()
        {
            if (!IsCorrectPhase) return;
            EndCoordination();
        }
    }
}
