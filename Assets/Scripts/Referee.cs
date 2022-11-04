using System.Collections;
using UnityEngine;

public class Referee : MonoBehaviour
{
    [SerializeField] private UnitTeam PlayerTeam;
    [SerializeField] private UnitTeam EnemyTeam;

    private void Start() { StartCoroutine(RunCombat()); }

    private IEnumerator RunCombat()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Run1v1());
    }

    private IEnumerator Run1v1()
    {
        Unit playerUnit = PlayerTeam.FrontUnit;
        Unit enemyUnit = EnemyTeam.FrontUnit;
        var isPlayerTurn = true;

        while (true)
        {
            Unit attacker = isPlayerTurn ? playerUnit : enemyUnit;
            Unit defender = isPlayerTurn ? enemyUnit : playerUnit;

            attacker.AttackUnit(defender);
            isPlayerTurn = !isPlayerTurn;

            yield return new WaitForSeconds(1f);
        }
    }
}
