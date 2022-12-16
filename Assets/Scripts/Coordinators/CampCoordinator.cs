using UnityEngine;
using UnityEngine.UI;

namespace GulchGuardians.Coordinators
{
    public class CampCoordinator : MonoBehaviour
    {
        [SerializeField] private GameObject Container;
        [SerializeField] private Button AdvanceButton;
        [SerializeField] private GameState State;

        public bool IsActive { get; private set; }
        private bool IsCorrectPhase => State.NightPhase == NightPhase.Camp;

        private void Awake() { Container.SetActive(false); }
        private void Start() { AdvanceButton.onClick.AddListener(OnAdvanceButtonClicked); }

        public void BeginCoordination()
        {
            Container.SetActive(true);
            IsActive = true;
        }

        private void EndCoordination()
        {
            IsActive = false;
            Container.SetActive(false);
        }

        private void OnAdvanceButtonClicked()
        {
            if (!IsCorrectPhase) return;
            EndCoordination();
        }
    }
}
