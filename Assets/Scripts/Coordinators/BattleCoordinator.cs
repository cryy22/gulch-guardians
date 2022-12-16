using System.Collections;
using GulchGuardians.Audio;
using GulchGuardians.Teams;
using GulchGuardians.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GulchGuardians.Coordinators
{
    public class BattleCoordinator : MonoBehaviour
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

        public bool IsActive { get; private set; }

        private void Awake() { _advanceButtonText = AdvanceButton.GetComponentInChildren<TMP_Text>(); }

        private void Start() { AdvanceButton.onClick.AddListener(OnAdvanceButtonClicked); }

        public void BeginCoordination()
        {
            IsActive = true;
            StartCoroutine(EnterPreparationPhase());
        }

        public void EndCoordination() { IsActive = false; }

        public void OnAdvanceInput(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (State.BattlePhase != BattlePhase.Preparation || PreparationCoordinator.IsActive) return;
            OnAdvance();
        }

        private void OnAdvanceButtonClicked()
        {
            if (State.BattlePhase != BattlePhase.Preparation) return;
            OnAdvance();
        }

        private void OnAdvance() { StartCoroutine(RunCombatPhase()); }

        private IEnumerator RunCombatPhase()
        {
            State.SetBattlePhase(BattlePhase.Transition);
            yield return PreparationCoordinator.EndModificationRound();

            BGMPlayer.Instance.TransitionToCombat();
            yield return GamePhaseAnnouncer.AnnouncePhase(isPreparation: false);

            _advanceButtonText.text = "next";
            AdvanceButton.interactable = false;

            State.SetBattlePhase(BattlePhase.Combat);
            yield return CombatCoordinator.RunCombat();

            bool isGameEnded = PlayerTeam.IsDefeated || EnemyTeam.IsDefeated;
            StartCoroutine(isGameEnded ? HandleBattleEnd() : EnterPreparationPhase());
        }

        private IEnumerator EnterPreparationPhase()
        {
            State.SetBattlePhase(BattlePhase.Transition);

            BGMPlayer.Instance.TransitionToPreparation();
            yield return GamePhaseAnnouncer.AnnouncePhase(isPreparation: true);

            _advanceButtonText.text = "fight!";
            AdvanceButton.interactable = true;

            PreparationCoordinator.BeginModificationRound();
            State.SetBattlePhase(BattlePhase.Preparation);
        }

        private IEnumerator HandleBattleEnd()
        {
            yield return GameResultPanel.DisplayResult(EnemyTeam.IsDefeated);
            EndCoordination();
        }
    }
}
