using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Scene = GulchGuardians.Constants.Scene;

namespace GulchGuardians
{
    public class Referee : MonoBehaviour
    {
        [SerializeField] private Team PlayerTeam;
        [SerializeField] private Team EnemyTeam;
        [SerializeField] private TeamModifier TeamModifier;

        [SerializeField] private Button AdvanceButton;
        [SerializeField] private Button AutoButton;
        [SerializeField] private TMP_Text TrySpacebarText;
        [SerializeField] private UIGameResultPanel GameResultPanel;

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
            TeamModifier.BeginModificationRound();
        }

        public void OnAdvance(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (_currentPhase != Phase.Combat) return;

            OnAdvanceButtonClicked();

            if (_hasUsedSpacebar) return;

            _hasUsedSpacebar = true;
            TrySpacebarText.gameObject.SetActive(false);
        }

        private void OnAutoButtonClicked()
        {
            _isAutoAdvance = !_isAutoAdvance;
            _autoButtonText.text = _isAutoAdvance ? "Auto: On" : "Auto: Off";
            TrySpacebarText.gameObject.SetActive(!_isAutoAdvance && !_hasUsedSpacebar);
        }

        private void OnAdvanceButtonClicked()
        {
            if (_currentPhase == Phase.Combat) _didPlayerAdvance = true;
            else if (_currentPhase == Phase.Preparation) EnterCombatPhase();
        }

        private void EnterCombatPhase()
        {
            _currentPhase = Phase.Combat;
            _advanceButtonText.text = "next";

            AdvanceButton.interactable = false;
            AutoButton.gameObject.SetActive(true);

            TrySpacebarText.gameObject.SetActive(!_hasUsedSpacebar && !_isAutoAdvance);

            BGMPlayer.Instance.TransitionToCombat();

            StartCoroutine(RunCombat());
        }

        private void EnterPreparationPhase()
        {
            _currentPhase = Phase.Preparation;
            _advanceButtonText.text = "fight!";

            AdvanceButton.interactable = true;
            AutoButton.gameObject.SetActive(false);

            TrySpacebarText.gameObject.SetActive(false);

            BGMPlayer.Instance.TransitionToPreparation();

            TeamModifier.BeginModificationRound();
        }

        private IEnumerator RunCombat()
        {
            PlayerTeam.ResetUnitsOnDeck();
            EnemyTeam.ResetUnitsOnDeck();

            yield return TeamModifier.EndModificationRound();

            yield return WaitForPlayer(1f);

            while (true)
            {
                if (PlayerTeam.UnitsInCombatCycle == 0 || EnemyTeam.UnitsInCombatCycle == 0) break;
                yield return RunAttackCycle(player: PlayerTeam, enemy: EnemyTeam);
            }

            if (PlayerTeam.Units.Count == 0 || EnemyTeam.Units.Count == 0)
            {
                yield return GameResultPanel.DisplayResult(PlayerTeam.Units.Count > 0);

                SceneManager.LoadScene(Scene.TitleScene);
                yield break;
            }

            PlayerTeam.ResetUnitsOnDeck();
            EnemyTeam.ResetUnitsOnDeck();

            EnterPreparationPhase();
        }

        private IEnumerator RunAttackCycle(Team player, Team enemy)
        {
            IEnumerable<Unit> playerAttackers = player.Units.Where((u, index) => u.WillAttack(index));
            foreach (Unit attacker in playerAttackers)
            {
                Unit defender = enemy.FrontUnit;

                yield return attacker.AttackUnit(target: defender, unitTeam: player);
                yield return WaitForPlayer();

                if (defender != null && !defender.IsDefeated) continue;

                yield return enemy.HandleUnitDefeat(defender);
                if (enemy.UnitsInCombatCycle <= 0) yield break;

                yield return WaitForPlayer(0.25f);
            }

            IEnumerable<Unit> enemyAttackers = enemy.Units.Where((u, index) => u.WillAttack(index));
            foreach (Unit attacker in enemyAttackers)
            {
                Unit defender = player.FrontUnit;

                yield return attacker.AttackUnit(target: defender, unitTeam: enemy);
                yield return WaitForPlayer();

                if (defender != null && !defender.IsDefeated) continue;

                yield return player.HandleUnitDefeat(defender);
                if (player.UnitsInCombatCycle <= 0) yield break;

                yield return WaitForPlayer(0.25f);
            }

            if (player.UnitsInCombatCycle <= 1) yield break;

            yield return player.SetUnitIndex(unit: player.FrontUnit, index: player.UnitsInCombatCycle - 1);
            yield return WaitForPlayer();
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
        }
    }
}
