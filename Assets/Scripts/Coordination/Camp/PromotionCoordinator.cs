using System;
using System.Collections;
using Crysc.Coordination;
using GulchGuardians.Abilities;
using GulchGuardians.Promotions;
using GulchGuardians.Teams;
using GulchGuardians.Units;
using TMPro;
using UnityEngine;

namespace GulchGuardians.Coordination.Camp
{
    public class PromotionCoordinator : Coordinator
    {
        [SerializeField] private Team PlayerTeam;
        [SerializeField] private PromotionChooser PromotionChooser;
        [SerializeField] private TMP_Text InstructionText;

        private Unit _selectedUnit;

        protected override void Awake()
        {
            base.Awake();

            InstructionText.gameObject.SetActive(false);
            PromotionChooser.gameObject.SetActive(false);
        }

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
            PromotionChooser.gameObject.SetActive(false);
            UnregisterFromEvents();

            base.EndCoordination();
        }

        private void RegisterForEvents()
        {
            foreach (Unit unit in PlayerTeam.Units) unit.Clicked += UnitClickedHandler;
            PromotionChooser.Selected += PromotionSelectedHandler;
        }

        private void UnregisterFromEvents()
        {
            foreach (Unit unit in PlayerTeam.Units) unit.Clicked -= UnitClickedHandler;
            PromotionChooser.Selected -= PromotionSelectedHandler;
        }

        private void UnitClickedHandler(object sender, EventArgs _)
        {
            _selectedUnit = sender as Unit;
            if (_selectedUnit == null) return;

            PromotionChooser.gameObject.SetActive(true);
        }

        private void PromotionSelectedHandler(object sender, PromotionSelectedEventArgs e)
        {
            StartCoroutine(ApplyPromotion(e.Ability));
        }

        private IEnumerator ApplyPromotion(AbilityType ability)
        {
            yield return _selectedUnit.AddAbility(ability);
            EndCoordination();
        }
    }
}
