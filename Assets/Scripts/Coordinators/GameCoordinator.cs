using System.Collections;
using GulchGuardians.Audio;
using GulchGuardians.Constants;
using GulchGuardians.Teams;
using GulchGuardians.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GulchGuardians.Coordinators
{
    public class GameCoordinator : MonoBehaviour
    {
        [SerializeField] private GameState State;
        [SerializeField] private PreparationCoordinator PreparationCoordinator;
        [SerializeField] private CombatCoordinator CombatCoordinator;
        [SerializeField] private Team PlayerTeam;
        [SerializeField] private Team EnemyTeam;

        [SerializeField] private Button AdvanceButton;
        [SerializeField] private UIGamePhaseAnnouncer GamePhaseAnnouncer;
        [SerializeField] private UIGameResultPanel GameResultPanel;

        private TMP_Text _advanceButtonText;

        private void Awake() { _advanceButtonText = AdvanceButton.GetComponentInChildren<TMP_Text>(); }

        private void Start()
        {
            AdvanceButton.onClick.AddListener(OnAdvanceButtonClicked);
            StartCoroutine(EnterPreparationPhase());
        }

        public void OnAdvanceInput(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (State.Phase != Phase.Preparation || PreparationCoordinator.IsRoundActive) return;
            OnAdvance();
        }

        private void OnAdvanceButtonClicked()
        {
            if (State.Phase != Phase.Preparation) return;
            OnAdvance();
        }

        private void OnAdvance() { StartCoroutine(RunCombatPhase()); }

        private IEnumerator RunCombatPhase()
        {
            State.SetPhase(Phase.Transition);
            yield return PreparationCoordinator.EndModificationRound();

            BGMPlayer.Instance.TransitionToCombat();
            yield return GamePhaseAnnouncer.AnnouncePhase(isPreparation: false);

            _advanceButtonText.text = "next";
            AdvanceButton.interactable = false;

            State.SetPhase(Phase.Combat);
            yield return CombatCoordinator.RunCombat();

            bool isGameEnded = PlayerTeam.IsDefeated || EnemyTeam.IsDefeated;
            StartCoroutine(isGameEnded ? HandleGameEnd() : EnterPreparationPhase());
        }

        private IEnumerator EnterPreparationPhase()
        {
            State.SetPhase(Phase.Transition);

            BGMPlayer.Instance.TransitionToPreparation();
            yield return GamePhaseAnnouncer.AnnouncePhase(isPreparation: true);

            _advanceButtonText.text = "fight!";
            AdvanceButton.interactable = true;

            PreparationCoordinator.BeginModificationRound();
            State.SetPhase(Phase.Preparation);
        }

        private IEnumerator HandleGameEnd()
        {
            yield return GameResultPanel.DisplayResult(EnemyTeam.IsDefeated);
            SceneManager.LoadScene(Scenes.TitleIndex);
        }
    }
}
