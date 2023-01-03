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
        private const string _reorderTeamText = "drag and drop to reorder the squad";

        [SerializeField] private Button AdvanceButton;
        [SerializeField] private TMP_Text InstructionText;
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
            AdvanceButton.gameObject.SetActive(true);
            AdvanceButton.interactable = true;
            InstructionText.gameObject.SetActive(true);

            StartCoroutine(PlayerTeam.View.RearrangeForCamp(PlayerTeamContainer));
            StartCoroutine(Run());
        }

        public override void EndCoordination()
        {
            PlayerTeam.FrontSquad.Reorderer.EndReordering();

            InstructionText.gameObject.SetActive(false);
            Background.SetCurtain(false);
            AdvanceButton.gameObject.SetActive(false);

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

            _advanceButtonText.text = "to battle!";
            InstructionText.text = _reorderTeamText;

            PlayerTeam.FrontSquad.Reorderer.BeginReordering();
        }
    }
}
