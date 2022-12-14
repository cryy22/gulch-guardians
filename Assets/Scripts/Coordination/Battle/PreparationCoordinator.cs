using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Coordination;
using Crysc.Helpers;
using GulchGuardians.ModificationEffects;
using GulchGuardians.Teams;
using GulchGuardians.Units;
using TMPro;
using UnityEngine;

namespace GulchGuardians.Coordination.Battle
{
    public class PreparationCoordinator : Coordinator
    {
        private const string _chooseEffectText = "choose an effect";
        private const string _chooseUnitText = "choose a unit to apply the effect";

        [SerializeField] private int ActionsPerRound = 2;
        [SerializeField] private List<ModificationEffect> Effects = new();

        [SerializeField] private Team PlayerTeam;
        [SerializeField] private Team EnemyTeam;

        [SerializeField] private EffectOptionsDisplayer EffectOptionsDisplayer;
        [SerializeField] private TMP_Text ActionsRemainingText;
        [SerializeField] private TMP_Text InstructionText;

        private readonly List<ModificationEffect> _offeredEffects = new();

        private int _actionsRemaining;
        private bool _isModifying;

        private void OnEnable()
        {
            EffectOptionsDisplayer.EffectSelected += EffectSelectedEventHandler;
            PlayerTeam.UnitClicked += UnitClickedEventHandler;
        }

        private void OnDisable()
        {
            EffectOptionsDisplayer.EffectSelected -= EffectSelectedEventHandler;
            PlayerTeam.UnitClicked -= UnitClickedEventHandler;
        }

        public override void BeginCoordination()
        {
            base.BeginCoordination();

            EffectOptionsDisplayer.gameObject.SetActive(true);
            _actionsRemaining = ActionsPerRound;
            UpdateActionsRemainingText();

            InstructionText.gameObject.SetActive(true);
            OfferEffectOptions();
        }

        public IEnumerator EndModificationRound()
        {
            yield return new WaitUntil(() => !_isModifying);

            CleanUpOfferedEffects();
            EffectOptionsDisplayer.CleanUpEffectOptions();
            EffectOptionsDisplayer.gameObject.SetActive(false);
            InstructionText.gameObject.SetActive(false);

            EndCoordination();
        }

        private void UnitClickedEventHandler(object sender, UnitClickedEventArgs e)
        {
            if (EffectOptionsDisplayer.SelectedEffect == null) return;
            StartCoroutine(ApplySelectedEffect(e.Unit));
        }

        private void EffectSelectedEventHandler(object sender, EventArgs e)
        {
            switch (EffectOptionsDisplayer.SelectedEffect.Target)
            {
                case ModificationEffect.TargetType.PlayerTeam:
                case ModificationEffect.TargetType.EnemyTeam:
                    StartCoroutine(ApplySelectedEffect(null));
                    break;
                default:
                    InstructionText.text = _chooseUnitText;
                    break;
            }
        }

        private IEnumerator ApplySelectedEffect(Unit unit)
        {
            yield return new WaitUntil(() => !_isModifying);
            _isModifying = true;

            yield return EffectOptionsDisplayer.SelectedEffect.Apply(BuildContext(unit));

            _actionsRemaining--;
            UpdateActionsRemainingText();

            CleanUpOfferedEffects();
            if (_actionsRemaining <= 0)
                StartCoroutine(EndModificationRound());
            else
                OfferEffectOptions();

            _isModifying = false;
        }

        private void OfferEffectOptions()
        {
            InstructionText.text = _chooseEffectText;

            _offeredEffects.AddRange(GetRandomEffects(3));
            foreach (ModificationEffect effect in _offeredEffects) effect.Prepare();

            EffectOptionsDisplayer.DisplayEffectOptions(_offeredEffects);
        }

        private void CleanUpOfferedEffects()
        {
            foreach (ModificationEffect effect in _offeredEffects) effect.CleanUp();
            _offeredEffects.Clear();
        }

        private IEnumerable<ModificationEffect> GetRandomEffects(int numberOfEffects)
        {
            List<ModificationEffect> chosenEffects = new();
            ModificationEffect.Context context = BuildContext();
            List<ModificationEffect> availableEffects =
                Effects.Where(e => e.CanBeAppliedTo(context)).ToList();

            while (chosenEffects.Count < numberOfEffects)
            {
                if (availableEffects.Count == 0) break;
                ModificationEffect effect = Randomizer.GetWeightedRandomElement(
                    enumerable: availableEffects,
                    e => e.RarityWeight
                );

                chosenEffects.Add(effect);
                availableEffects.Remove(effect);
            }

            return chosenEffects;
        }

        private void UpdateActionsRemainingText()
        {
            ActionsRemainingText.text = $"actions remaining: {_actionsRemaining.ToString()}";
        }

        private ModificationEffect.Context BuildContext(Unit unit = null)
        {
            return new ModificationEffect.Context
            {
                Unit = unit,
                PlayerTeam = PlayerTeam,
                EnemyTeam = EnemyTeam,
            };
        }
    }
}
