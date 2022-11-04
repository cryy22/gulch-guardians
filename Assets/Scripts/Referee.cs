using System.Collections;
using DefaultNamespace;
using UnityEngine;

public class Referee : MonoBehaviour
{
    [SerializeField] private UnitTeam PlayerTeam;
    [SerializeField] private UnitTeam EnemyTeam;
    [SerializeField] private UIGameResultPanel GameResultPanel;

    private bool _isPlayerTurn = true;

    private void Start() { StartCoroutine(RunCombat()); }

    private IEnumerator RunCombat()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            if (PlayerTeam.UnitsInCombatCycle == 0 || EnemyTeam.UnitsInCombatCycle == 0) break;
            yield return StartCoroutine(Run1V1(player: PlayerTeam, enemy: EnemyTeam));
        }

        GameResultPanel.DisplayResult(isWin: PlayerTeam.UnitsInCombatCycle > 0);
    }

    private IEnumerator Run1V1(UnitTeam player, UnitTeam enemy)
    {
        UnitTeam attacker;
        UnitTeam defender;

        while (true)
        {
            attacker = _isPlayerTurn ? player : enemy;
            defender = _isPlayerTurn ? enemy : player;

            Unit attackerUnit = attacker.FrontUnit;
            Unit defenderUnit = defender.FrontUnit;

            bool result = attackerUnit.AttackUnit(defenderUnit);
            _isPlayerTurn = !_isPlayerTurn;

            if (result == true) break;
            yield return new WaitForSeconds(1f);
        }

        defender.UnitDefeated();
        yield return new WaitForSeconds(1f);
    }
}
