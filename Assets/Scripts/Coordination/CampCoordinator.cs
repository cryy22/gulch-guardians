using Crysc.Coordination;
using Crysc.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GulchGuardians.Coordination
{
    public class CampCoordinator : Coordinator
    {
        [SerializeField] private Button AdvanceButton;
        [SerializeField] private UIParallaxBackground Background;
        [SerializeField] private GameState State;

        private TMP_Text _advanceButtonText;

        private bool IsCorrectPhase => State.NightPhase == NightPhase.Camp;

        protected override void Awake() { _advanceButtonText = AdvanceButton.GetComponentInChildren<TMP_Text>(); }

        private void Start() { AdvanceButton.onClick.AddListener(OnAdvanceButtonClicked); }

        public override void BeginCoordination()
        {
            base.BeginCoordination();

            _advanceButtonText.text = "battle";
            AdvanceButton.interactable = true;
            Background.SetCurtain(true);
        }

        public override void EndCoordination()
        {
            Background.SetCurtain(false);

            base.EndCoordination();
        }

        private void OnAdvanceButtonClicked()
        {
            if (!IsCorrectPhase) return;
            EndCoordination();
        }
    }
}
