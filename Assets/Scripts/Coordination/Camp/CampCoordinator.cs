using System.Collections;
using Crysc.Coordination;
using Crysc.Presentation;
using GulchGuardians.Teams;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GulchGuardians.Coordination.Camp
{
    public class CampCoordinator : Coordinator
    {
        [SerializeField] private Button AdvanceButton;
        [SerializeField] private Transform PlayerTeamContainer;
        [SerializeField] private Team PlayerTeam;
        [SerializeField] private ParallaxBackground Background;

        [SerializeField] private GameState State;
        [SerializeField] private PromotionCoordinator PromotionCoordinator;

        private TMP_Text _advanceButtonText;

        protected override void Awake()
        {
            base.Awake();
            _advanceButtonText = AdvanceButton.GetComponentInChildren<TMP_Text>();
        }

        private void Start() { AdvanceButton.onClick.AddListener(OnAdvanceButtonClicked); }

        public override void BeginCoordination()
        {
            base.BeginCoordination();

            Background.SetCurtain(true);

            StartCoroutine(PlayerTeam.View.RearrangeForCamp(PlayerTeamContainer));
            StartCoroutine(Run());
        }

        public override void EndCoordination()
        {
            PlayerTeam.FrontSquad.Reorderer.EndReordering();
            Background.SetCurtain(false);

            base.EndCoordination();
        }

        private void OnAdvanceButtonClicked()
        {
            if (State.GamePhase != GamePhase.Camp) return;

            switch (State.CampPhase)
            {
                case CampPhase.Promotion:
                    PromotionCoordinator.EndCoordination();
                    break;
                case CampPhase.Reorder:
                    EndCoordination();
                    break;
            }
        }

        private IEnumerator Run()
        {
            yield return RunPromotionPhase();
            RunReorderPhase();
        }

        private IEnumerator RunPromotionPhase()
        {
            State.SetCampPhase(CampPhase.Promotion);

            _advanceButtonText.text = "skip promo";
            PromotionCoordinator.BeginCoordination();

            yield return new WaitUntil(() => !PromotionCoordinator.IsActive);
        }

        private void RunReorderPhase()
        {
            State.SetCampPhase(CampPhase.Reorder);

            _advanceButtonText.text = "battle!";
            PlayerTeam.FrontSquad.Reorderer.BeginReordering();
        }
    }
}
