using System.Collections;
using UnityEngine;
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

        [SerializeField] private Button RunCombatButton;
        [SerializeField] private UIGameResultPanel GameResultPanel;

        private bool _isPlayerTurn = true;

        private IEnumerator Start()
        {
            RunCombatButton.onClick.AddListener(OnRunCombatButtonClicked);

            yield return new WaitForSeconds(1f);
            TeamModifier.BeginModificationRound();
        }

        private void OnRunCombatButtonClicked()
        {
            RunCombatButton.gameObject.SetActive(false);
            StartCoroutine(RunCombat());
        }

        private IEnumerator RunCombat()
        {
            _isPlayerTurn = true;

            PlayerTeam.ResetUnitsOnDeck();
            EnemyTeam.ResetUnitsOnDeck();

            TeamModifier.EndModificationRound();

            yield return new WaitForSeconds(1f);

            while (true)
            {
                if (PlayerTeam.UnitsInCombatCycle == 0 || EnemyTeam.UnitsInCombatCycle == 0) break;
                yield return Run1V1(player: PlayerTeam, enemy: EnemyTeam);
            }

            bool isWin = PlayerTeam.UnitsInCombatCycle > 0;
            yield return GameResultPanel.DisplayResult(isWin);

            if (!isWin)
            {
                SceneManager.LoadScene(Scene.TitleScene);
                yield break;
            }

            PlayerTeam.ResetUnitsOnDeck();
            EnemyTeam.ResetUnitsOnDeck();

            TeamModifier.BeginModificationRound();
            RunCombatButton.gameObject.SetActive(true);
        }

        private IEnumerator Run1V1(Team player, Team enemy)
        {
            Team attacker;
            Team defender;

            while (true)
            {
                attacker = _isPlayerTurn ? player : enemy;
                defender = _isPlayerTurn ? enemy : player;

                Unit attackerUnit = attacker.FrontUnit;
                Unit defenderUnit = defender.FrontUnit;

                yield return attackerUnit.AttackUnit(defenderUnit);
                _isPlayerTurn = !_isPlayerTurn;
                if (defenderUnit.Health <= 0)
                {
                    yield return defenderUnit.BecomeDefeated();
                    break;
                }

                yield return new WaitForSeconds(1f);
            }

            defender.UnitDefeated();
            yield return new WaitForSeconds(1f);
        }
    }
}
