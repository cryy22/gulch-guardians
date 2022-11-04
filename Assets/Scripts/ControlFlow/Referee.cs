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

            yield return TeamModifier.EndModificationRound();

            BGMPlayer.Instance.TransitionToCombat();

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

            BGMPlayer.Instance.TransitionToPreparation();
        }

        private IEnumerator RunAttackCycle(Team player, Team enemy)
        {
            yield return player.FrontUnit.AttackUnit(enemy.FrontUnit);
            if (enemy.FrontUnit.IsDefeated) yield return enemy.HandleUnitDefeat(enemy.FrontUnit);

            if (enemy.UnitsInCombatCycle <= 0) yield break;

            yield return new WaitForSeconds(1f);

            Unit playerFrontUnit = player.FrontUnit;
            yield return enemy.FrontUnit.AttackUnit(playerFrontUnit);
            if (playerFrontUnit.IsDefeated) yield return player.HandleUnitDefeat(playerFrontUnit);

            if (player.UnitsInCombatCycle <= 0) yield break;

            yield return new WaitForSeconds(1f);

            if (playerFrontUnit == null || playerFrontUnit.IsDefeated) yield break;

            yield return player.SetUnitIndex(unit: player.FrontUnit, index: player.UnitsInCombatCycle - 1);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
