using System.Collections;
using UnityEngine;

namespace GulchGuardians.Coordinators
{
    public class GameCoordinator : MonoBehaviour
    {
        [SerializeField] private GameState State;
        [SerializeField] private BattleCoordinator BattleCoordinator;
        [SerializeField] private CampCoordinator CampCoordinator;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            yield return RunBattlePhase();
            yield return RunCampPhase();
        }

        private IEnumerator RunBattlePhase()
        {
            State.SetNightPhase(NightPhase.Battle);
            BattleCoordinator.BeginCoordination();
            yield return new WaitUntil(() => !BattleCoordinator.IsActive);
        }

        private IEnumerator RunCampPhase()
        {
            State.SetNightPhase(NightPhase.Camp);
            CampCoordinator.BeginCoordination();
            yield return new WaitUntil(() => !CampCoordinator.IsActive);
        }
    }
}
