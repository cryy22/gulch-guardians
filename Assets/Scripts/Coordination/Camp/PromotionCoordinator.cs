using System;
using Crysc.Coordination;
using GulchGuardians.Teams;
using GulchGuardians.Units;
using TMPro;
using UnityEngine;

namespace GulchGuardians.Coordination.Camp
{
    public class PromotionCoordinator : Coordinator
    {
        [SerializeField] private TMP_Text InstructionText;
        [SerializeField] private Team PlayerTeam;

        private void OnEnable()
        {
            if (IsActive) RegisterForEvents();
        }

        private void OnDisable()
        {
            if (IsActive) UnregisterFromEvents();
        }

        public override void BeginCoordination()
        {
            base.BeginCoordination();

            RegisterForEvents();
            InstructionText.gameObject.SetActive(true);
        }

        public override void EndCoordination()
        {
            InstructionText.gameObject.SetActive(false);
            UnregisterFromEvents();

            base.EndCoordination();
        }

        private void RegisterForEvents()
        {
            foreach (Unit unit in PlayerTeam.Units) unit.Clicked += UnitClickedHandler;
        }

        private void UnregisterFromEvents()
        {
            foreach (Unit unit in PlayerTeam.Units) unit.Clicked -= UnitClickedHandler;
        }

        private void UnitClickedHandler(object sender, EventArgs _)
        {
            var unit = sender as Unit;
            if (unit == null) return;

            Debug.Log($"unit clicked: {unit.FirstName}");
        }
    }
}
