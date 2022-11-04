using System.Collections;
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
        [SerializeField] private TMP_Text TrySpacebarText;
        [SerializeField] private UIGameResultPanel GameResultPanel;

        private TMP_Text _advanceButtonText;
        private Phase _currentPhase;
        private bool _didPlayerAdvance;
        private bool _hasUsedSpacebar;

        private void Awake() { _advanceButtonText = AdvanceButton.GetComponentInChildren<TMP_Text>(); }

        private IEnumerator Start()
        {
            AdvanceButton.onClick.AddListener(OnAdvanceButtonClicked);

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

            TrySpacebarText.gameObject.SetActive(!_hasUsedSpacebar);

            BGMPlayer.Instance.TransitionToCombat();

            StartCoroutine(RunCombat());
        }

        private void EnterPreparationPhase()
        {
            _currentPhase = Phase.Preparation;
            _advanceButtonText.text = "fight!";
            AdvanceButton.interactable = true;

            TrySpacebarText.gameObject.SetActive(false);

            BGMPlayer.Instance.TransitionToPreparation();

            TeamModifier.BeginModificationRound();
        }

        private IEnumerator RunCombat()
        {
            PlayerTeam.ResetUnitsOnDeck();
            EnemyTeam.ResetUnitsOnDeck();

            yield return TeamModifier.EndModificationRound();

            yield return new WaitForSeconds(1f);

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
            Unit playerUnit = player.FrontUnit;
            Unit enemyUnit = enemy.FrontUnit;

            yield return playerUnit.AttackUnit(enemyUnit);
            yield return WaitForPlayer();

            if (enemyUnit == null || enemyUnit.IsDefeated)
            {
                yield return enemy.HandleUnitDefeat(enemyUnit);
                if (enemy.UnitsInCombatCycle <= 0) yield break;
                yield return new WaitForSeconds(0.25f);

                enemyUnit = enemy.FrontUnit;
            }

            yield return enemyUnit.AttackUnit(playerUnit);
            yield return WaitForPlayer();

            if (playerUnit == null || playerUnit.IsDefeated)
            {
                yield return player.HandleUnitDefeat(playerUnit);
                yield break;
            }

            if (player.UnitsInCombatCycle <= 1) yield break;

            yield return player.SetUnitIndex(unit: player.FrontUnit, index: player.UnitsInCombatCycle - 1);
            yield return WaitForPlayer();
        }

        private IEnumerator WaitForPlayer()
        {
            AdvanceButton.interactable = true;
            yield return new WaitUntil(() => _didPlayerAdvance);

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
