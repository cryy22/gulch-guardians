using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GulchGuardians.Abilities;
using GulchGuardians.Audio;
using GulchGuardians.Constants;
using GulchGuardians.Squads;
using GulchGuardians.Teams;
using GulchGuardians.UI;
using GulchGuardians.Units;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GulchGuardians.Coordinators
{
    public class GameCoordinator : MonoBehaviour
    {
        [SerializeField] private Team PlayerTeam;
        [SerializeField] private Team EnemyTeam;
        [SerializeField] private PreparationCoordinator PreparationCoordinator;
        [SerializeField] private Button AdvanceButton;
        [SerializeField] private Button AutoButton;
        [SerializeField] private TMP_Text TrySpacebarText;
        [SerializeField] private UIGamePhaseAnnouncer GamePhaseAnnouncer;
        [SerializeField] private UIGameResultPanel GameResultPanel;

        [SerializeField] private AbilityType Evasive;

        private TMP_Text _advanceButtonText;
        private TMP_Text _autoButtonText;
        private Phase _currentPhase;
        private bool _didPlayerAdvance;
        private bool _hasUsedSpacebar;
        private bool _isAutoAdvance;

        private void Awake()
        {
            _advanceButtonText = AdvanceButton.GetComponentInChildren<TMP_Text>();
            _autoButtonText = AutoButton.GetComponentInChildren<TMP_Text>();
        }

        private IEnumerator Start()
        {
            AdvanceButton.onClick.AddListener(OnAdvanceButtonClicked);
            AutoButton.onClick.AddListener(OnAutoButtonClicked);

            yield return new WaitForSeconds(1f);
            PreparationCoordinator.BeginModificationRound();
        }

        public void OnAdvance(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (_currentPhase == Phase.Transition || PreparationCoordinator.IsRoundActive) return;

            OnAdvanceButtonClicked();

            if (_hasUsedSpacebar) return;

            _hasUsedSpacebar = true;
            TrySpacebarText.gameObject.SetActive(false);
        }

        private IEnumerator RotateSquad(Squad squad, bool withHurtAnimation = false)
        {
            if (squad.Count <= 1) yield break;

            yield return squad.SetUnitIndex(
                unit: squad.FrontUnit,
                index: squad.Count - 1,
                withHurtAnimation: withHurtAnimation
            );
            yield return WaitForPlayer();
        }

        private void OnAutoButtonClicked()
        {
            _isAutoAdvance = !_isAutoAdvance;
            _autoButtonText.text = _isAutoAdvance ? "Auto: On" : "Auto: Off";
            TrySpacebarText.gameObject.SetActive(!_isAutoAdvance && !_hasUsedSpacebar);
        }

        private void OnAdvanceButtonClicked()
        {
            switch (_currentPhase)
            {
                case Phase.Transition:
                    return;
                case Phase.Combat:
                    _didPlayerAdvance = true;
                    break;
                case Phase.Preparation:
                    StartCoroutine(EnterCombatPhase());
                    break;
            }
        }

        private IEnumerator EnterCombatPhase()
        {
            yield return PreparationCoordinator.EndModificationRound();
            _currentPhase = Phase.Transition;

            BGMPlayer.Instance.TransitionToCombat();
            yield return GamePhaseAnnouncer.AnnouncePhase(isPreparation: false);

            _advanceButtonText.text = "next";
            AdvanceButton.interactable = false;

            AutoButton.gameObject.SetActive(true);
            TrySpacebarText.gameObject.SetActive(!_hasUsedSpacebar && !_isAutoAdvance);

            StartCoroutine(RunCombat());
            _currentPhase = Phase.Combat;
        }

        private IEnumerator EnterPreparationPhase()
        {
            _currentPhase = Phase.Transition;

            BGMPlayer.Instance.TransitionToPreparation();
            yield return GamePhaseAnnouncer.AnnouncePhase(isPreparation: true);

            _advanceButtonText.text = "fight!";
            AdvanceButton.interactable = true;

            AutoButton.gameObject.SetActive(false);
            TrySpacebarText.gameObject.SetActive(false);

            PreparationCoordinator.BeginModificationRound();
            _currentPhase = Phase.Preparation;
        }

        private IEnumerator RunCombat()
        {
            PlayerTeam.ResetUnitsOnDeck();
            EnemyTeam.ResetUnitsOnDeck();

            Squad playerSquad = PlayerTeam.FrontSquad;
            Squad enemySquad = EnemyTeam.FrontSquad;

            yield return WaitForPlayer(1f);

            while (true)
            {
                if (Squad.IsDefeated(playerSquad) || Squad.IsDefeated(enemySquad)) break;
                yield return RunAttackCycle(playerSquad: playerSquad, enemySquad: enemySquad);
            }

            if (PlayerTeam.Units.Count() <= 0 || EnemyTeam.Units.Count() <= 0)
            {
                yield return GameResultPanel.DisplayResult(PlayerTeam.Units.Count() > 0);

                SceneManager.LoadScene(Scenes.TitleIndex);
                yield break;
            }

            PlayerTeam.ResetUnitsOnDeck();
            EnemyTeam.ResetUnitsOnDeck();

            yield return EnterPreparationPhase();
        }

        private IEnumerator RunAttackCycle(Squad playerSquad, Squad enemySquad)
        {
            yield return RunSquadAttack(attackingSquad: playerSquad, defendingSquad: enemySquad);

            if (enemySquad.Count <= 0) yield break;
            if (playerSquad.Count <= 0) yield break;

            yield return RunSquadAttack(attackingSquad: enemySquad, defendingSquad: playerSquad);
        }

        private IEnumerator RunSquadAttack(Squad attackingSquad, Squad defendingSquad)
        {
            IEnumerable<Unit> attackers = attackingSquad.Units.Where((u, index) => u.WillAttack(index));
            foreach (Unit attacker in attackers)
            {
                yield return RotateEvasiveAwayFromFront(defendingSquad);
                Unit defender = defendingSquad.FrontUnit;

                yield return attacker.AttackUnit(target: defender);
                yield return WaitForPlayer();

                if (defender != null && !defender.IsDefeated)
                {
                    yield return RotateSquad(squad: defendingSquad, withHurtAnimation: true);
                    continue;
                }

                yield return defendingSquad.HandleUnitDefeat(defender);

                if (defendingSquad.Count <= 0) yield break;
                yield return WaitForPlayer();
            }
        }

        private IEnumerator RotateEvasiveAwayFromFront(Squad squad)
        {
            int evasiveCount = squad.Units.Count(u => u.HasAbility(Evasive));
            if (evasiveCount == 0 || evasiveCount == squad.Count) yield break;

            while (squad.FrontUnit.HasAbility(Evasive))
                yield return RotateSquad(squad: squad, withHurtAnimation: false);
        }

        private IEnumerator WaitForPlayer(float autoAdvanceDelay = 0f)
        {
            AdvanceButton.interactable = true;
            yield return new WaitUntil(() => _didPlayerAdvance || _isAutoAdvance);
            if (_isAutoAdvance) yield return new WaitForSeconds(autoAdvanceDelay);

            AdvanceButton.interactable = false;
            _didPlayerAdvance = false;
        }

        private enum Phase
        {
            Preparation,
            Combat,
            Transition,
        }
    }
}
