using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace InfiniteSAPPrototype
{
    public class TeamModifier : MonoBehaviour
    {
        [SerializeField] private int NumberOfUnitsPerRound = 3;
        [SerializeField] private int ActionsPerRound = 2;

        [SerializeField] private UnitFactory UnitFactory;
        [SerializeField] private UnitTeam PlayerTeam;
        [SerializeField] private Transform UnitList;

        [SerializeField] private TMP_Text ActionsRemainingText;

        private readonly List<Unit> _newUnitOptions = new();
        private int _actionsRemaining;

        public void BeginModificationRound()
        {
            gameObject.SetActive(true);
            _actionsRemaining = ActionsPerRound;
            UpdateActionsRemainingText();

            for (var i = 0; i < NumberOfUnitsPerRound; i++)
            {
                Unit unit = UnitFactory.Create();
                _newUnitOptions.Add(unit);
                unit.transform.SetParent(UnitList);

                unit.GetComponent<ClickReporter>().OnReporterClickedEvent += OnNewUnitOptionClicked;
            }

            foreach (Unit unit in PlayerTeam.Units)
                unit.GetComponent<ClickReporter>().OnReporterClickedEvent += OnExistingUnitClicked;
        }

        public void EndModificationRound()
        {
            _newUnitOptions.ForEach(unit => Destroy(unit.gameObject));
            _newUnitOptions.Clear();

            foreach (Unit unit in PlayerTeam.Units)
                unit.GetComponent<ClickReporter>().OnReporterClickedEvent -= OnExistingUnitClicked;

            gameObject.SetActive(false);
        }

        private void OnNewUnitOptionClicked(ClickReporter reporter)
        {
            var selectedUnit = reporter.GetComponent<Unit>();
            PlayerTeam.AddUnit(selectedUnit);

            reporter.OnReporterClickedEvent -= OnNewUnitOptionClicked;
            reporter.OnReporterClickedEvent += OnExistingUnitClicked;

            _newUnitOptions.Remove(selectedUnit);
            DecrementActions();
        }

        private void OnExistingUnitClicked(ClickReporter reporter)
        {
            var selectedUnit = reporter.GetComponent<Unit>();
            selectedUnit.Upgrade(attack: 1, health: 1);

            DecrementActions();
        }

        private void DecrementActions()
        {
            _actionsRemaining--;
            UpdateActionsRemainingText();
            if (_actionsRemaining <= 0) EndModificationRound();
        }

        private void UpdateActionsRemainingText()
        {
            ActionsRemainingText.text = $"actions remaining: {_actionsRemaining}";
        }
    }
}
