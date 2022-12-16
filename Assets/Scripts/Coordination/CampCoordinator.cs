using Crysc.Coordination;
using UnityEngine;
using UnityEngine.UI;

namespace GulchGuardians.Coordination
{
    public class CampCoordinator : Coordinator
    {
        [SerializeField] private Button AdvanceButton;
        [SerializeField] private GameState State;

        private bool IsCorrectPhase => State.NightPhase == NightPhase.Camp;

        private void Start() { AdvanceButton.onClick.AddListener(OnAdvanceButtonClicked); }

        private void OnAdvanceButtonClicked()
        {
            if (!IsCorrectPhase) return;
            EndCoordination();
        }
    }
}
