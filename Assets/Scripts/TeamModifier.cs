using System.Collections.Generic;
using UnityEngine;

namespace InfiniteSAPPrototype
{
    public class TeamModifier : MonoBehaviour
    {
        [SerializeField] private int NumberOfUnitsPerRound = 3;

        [SerializeField] private UnitFactory UnitFactory;
        [SerializeField] private UnitTeam PlayerTeam;
        [SerializeField] private Transform UnitList;

        private readonly List<Unit> _newUnitOptions = new();

        public void BeginModificationRound()
        {
            gameObject.SetActive(true);
            for (var i = 0; i < NumberOfUnitsPerRound; i++)
            {
                Unit unit = UnitFactory.Create();
                _newUnitOptions.Add(unit);
                unit.transform.SetParent(UnitList);

                unit.GetComponent<ClickReporter>().OnReporterClickedEvent += OnUnitClicked;
            }
        }

        private void OnUnitClicked(ClickReporter reporter)
        {
            var selectedUnit = reporter.GetComponent<Unit>();
            PlayerTeam.AddUnit(selectedUnit);

            _newUnitOptions.Remove(selectedUnit);
            _newUnitOptions.ForEach(unit => Destroy(unit.gameObject));
            _newUnitOptions.Clear();

            gameObject.SetActive(false);
        }
    }
}
