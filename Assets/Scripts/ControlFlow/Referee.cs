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
            PlayerTeam.ResetUnitsOnDeck();
            EnemyTeam.ResetUnitsOnDeck();

            TeamModifier.EndModificationRound();

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

            TeamModifier.BeginModificationRound();
            RunCombatButton.gameObject.SetActive(true);
        }

        private IEnumerator RunAttackCycle(Team player, Team enemy)
        {
            yield return player.FrontUnit.AttackUnit(enemy.FrontUnit);
            if (enemy.FrontUnit.Health <= 0)
            {
                yield return enemy.UnitDefeated(enemy.FrontUnit);
                if (enemy.UnitsInCombatCycle <= 0) yield break;
            }

            yield return new WaitForSeconds(1f);

            var isPlayerUnitDefeated = false;

            yield return enemy.FrontUnit.AttackUnit(player.FrontUnit);
            if (player.FrontUnit.Health <= 0)
            {
                yield return player.UnitDefeated(player.FrontUnit);
                isPlayerUnitDefeated = true;
                if (player.UnitsInCombatCycle <= 0) yield break;
            }

            yield return new WaitForSeconds(1f);

            if (isPlayerUnitDefeated) yield break;

            player.SetUnitIndex(unit: player.FrontUnit, index: player.UnitsInCombatCycle - 1);
            yield return null; // let the UI update

            yield return new WaitForSeconds(0.5f);
        }
    }
}
