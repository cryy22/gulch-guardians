using Crysc.Coordination;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GulchGuardians.Coordination
{
    public class CampCoordinator : Coordinator
    {
        [SerializeField] private Button AdvanceButton;
        [SerializeField] private GameState State;

        private TMP_Text _advanceButtonText;

        private bool IsCorrectPhase => State.NightPhase == NightPhase.Camp;

        private void Awake() { _advanceButtonText = AdvanceButton.GetComponentInChildren<TMP_Text>(); }

        private void Start() { AdvanceButton.onClick.AddListener(OnAdvanceButtonClicked); }

        public override void BeginCoordination()
        {
            base.BeginCoordination();

            _advanceButtonText.text = "battle";
            AdvanceButton.interactable = true;
        }

        private void OnAdvanceButtonClicked()
        {
            if (!IsCorrectPhase) return;
            EndCoordination();
        }
    }
}
