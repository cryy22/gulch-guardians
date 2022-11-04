using System.Collections;
using GulchGuardians;
using UnityEngine;
using UnityEngine.UI;

public class Referee : MonoBehaviour
{
    [SerializeField] private UnitTeam PlayerTeam;
    [SerializeField] private UnitTeam EnemyTeam;
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

        yield return GameResultPanel.DisplayResult(isWin: PlayerTeam.UnitsInCombatCycle > 0);

        PlayerTeam.ResetUnitsOnDeck();
        EnemyTeam.ResetUnitsOnDeck();

        TeamModifier.BeginModificationRound();
        RunCombatButton.gameObject.SetActive(true);
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
