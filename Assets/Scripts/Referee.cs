using System.Collections;
using DefaultNamespace;
using UnityEngine;

public class Referee : MonoBehaviour
{
    [SerializeField] private UnitTeam PlayerTeam;
    [SerializeField] private UnitTeam EnemyTeam;
    [SerializeField] private UIGameResultPanel GameResultPanel;

    private void Start() { StartCoroutine(RunCombat()); }

    private IEnumerator RunCombat()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            Unit playerUnit = PlayerTeam.FrontUnit;
            Unit enemyUnit = EnemyTeam.FrontUnit;
            if (playerUnit == null || enemyUnit == null) break;

            yield return StartCoroutine(Run1V1(playerUnit: playerUnit, enemyUnit: enemyUnit));
        }

        GameResultPanel.DisplayResult(isWin: PlayerTeam.FrontUnit != null);
    }

    private IEnumerator Run1V1(Unit playerUnit, Unit enemyUnit)
    {
        var isPlayerTurn = true;

        while (playerUnit != null && enemyUnit != null)
        {
            Unit attacker = isPlayerTurn ? playerUnit : enemyUnit;
            Unit defender = isPlayerTurn ? enemyUnit : playerUnit;

            attacker.AttackUnit(defender);
            isPlayerTurn = !isPlayerTurn;

            yield return new WaitForSeconds(1f);
        }

        Debug.Log("1v1 over");
    }
}
