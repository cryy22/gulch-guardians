using System.Collections;
using Crysc.Coordination;
using GulchGuardians.Constants;
using GulchGuardians.Teams;
using GulchGuardians.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GulchGuardians.Coordination
{
    public class GameCoordinator : Coordinator
    {
        [SerializeField] private GameState State;
        [SerializeField] private BattleCoordinator BattleCoordinator;
        [SerializeField] private CampCoordinator CampCoordinator;

        [SerializeField] private Team PlayerTeam;
        [SerializeField] private GameResultPanel GameResultPanel;

        [SerializeField] private bool BeginInCampPhase;

        private bool IsGameEnded => IsGameLost || IsGameWon;
        private bool IsGameWon => State.Night >= BattleCoordinator.EnemyPlatoonCount;
        private bool IsGameLost => PlayerTeam.IsDefeated;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            BeginCoordination();

            BattleCoordinator.PopulatePlayerTeam();

            if (BeginInCampPhase) yield return RunCampPhase();
            yield return RunBattlePhase();

            while (!IsGameEnded)
            {
                yield return RunCampPhase();
                yield return RunBattlePhase();
            }

            yield return HandleGameEnd();
            EndCoordination();
        }

        private IEnumerator RunBattlePhase()
        {
            State.SetNightPhase(NightPhase.Battle);

            BattleCoordinator.BeginCoordination();
            yield return new WaitUntil(() => !BattleCoordinator.IsActive);

            State.IncrementNight();
        }

        private IEnumerator RunCampPhase()
        {
            State.SetNightPhase(NightPhase.Camp);

            CampCoordinator.BeginCoordination();
            yield return new WaitUntil(() => !CampCoordinator.IsActive);
        }

        private IEnumerator HandleGameEnd()
        {
            yield return GameResultPanel.DisplayResult(IsGameWon);
            SceneManager.LoadScene(Scenes.TitleIndex);
        }
    }
}
