using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GulchGuardians
{
    public class Team : MonoBehaviour
    {
        public List<Unit> Units = new();
        public int UnitsPerCombatCycle = 3;

        [SerializeField] private bool IsPlayerTeam;
        [SerializeField] private int UnitCount = 3;
        [SerializeField] private bool DemarcatesRounds;

        [SerializeField] private UnitFactory UnitFactory;
        [SerializeField] private GameObject RoundDemarcation;

        public event EventHandler UnitsChanged;

        public Unit FrontUnit => Units.Count > 0 ? Units.First() : null;
        public int UnitsInCombatCycle { get; private set; }

        private void Start() // should probably be Awake
        {
            RoundDemarcation.SetActive(DemarcatesRounds);

            for (var i = 0; i < UnitCount; i++)
            {
                Unit unit = UnitFactory.Create(isPlayerTeam: IsPlayerTeam);
                AddUnit(unit);
            }

            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);

            ResetUnitsOnDeck();
        }

        public void UnitDefeated()
        {
            Units.RemoveAt(0);
            UnitsInCombatCycle--;
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);

            UpdateDemarcation();
        }

        public void ResetUnitsOnDeck()
        {
            UnitsInCombatCycle = Mathf.Min(a: UnitsPerCombatCycle, b: Units.Count);
            UpdateDemarcation();
        }

        public void AddUnit(Unit unit)
        {
            Units.Add(unit);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void SetUnitIndex(Unit unit, int index)
        {
            if (!Units.Remove(unit)) return;

            Units.Insert(index: index, item: unit);
            UnitsChanged?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private void UpdateDemarcation()
        {
            if (!DemarcatesRounds) return;

            RoundDemarcation.SetActive(UnitsInCombatCycle > 0);
            if (UnitsInCombatCycle == 0) return;

            Transform lastUnitInCombatCycle = Units[UnitsInCombatCycle - 1].transform;

            Vector3 initialPosition = RoundDemarcation.transform.position;
            RoundDemarcation.transform.position = new Vector3(
                x: lastUnitInCombatCycle.position.x + 1f,
                y: initialPosition.y,
                z: initialPosition.z
            );
        }
    }
}
