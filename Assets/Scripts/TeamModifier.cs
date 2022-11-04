using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GulchGuardians
{
    [RequireComponent(typeof(EffectOptionsDisplayer))]
    public class TeamModifier : MonoBehaviour
    {
        [SerializeField] private int NumberOfUnitsPerRound = 1;
        [SerializeField] private int ActionsPerRound = 2;
        [SerializeField] private List<ModificationEffect> Effects = new();

        [SerializeField] private UnitFactory UnitFactory;
        [SerializeField] private Team PlayerTeam;

        [SerializeField] private TMP_Text ActionsRemainingText;
        [SerializeField] private TMP_Text ChooseATargetText;
        private readonly List<ModificationEffect> _offeredEffects = new();

        private int _actionsRemaining;
        private EffectOptionsDisplayer _effectOptions;

        private void Awake() { _effectOptions = GetComponent<EffectOptionsDisplayer>(); }
        private void OnEnable() { _effectOptions.EffectSelected += EffectSelectedEventHandler; }
        private void OnDisable() { _effectOptions.EffectSelected -= EffectSelectedEventHandler; }

        public void BeginModificationRound()
        {
            gameObject.SetActive(true);
            _actionsRemaining = ActionsPerRound;
            UpdateActionsRemainingText();

            for (var i = 0; i < NumberOfUnitsPerRound; i++)
            {
                Unit unit = UnitFactory.Create(isPlayerTeam: true);
                PlayerTeam.AddUnit(unit);
            }

            OfferEffectOptions();

            foreach (Unit unit in PlayerTeam.Units)
                unit.GetComponent<ClickReporter>().OnReporterClickedEvent += OnUnitClicked;
        }

        public void EndModificationRound()
        {
            foreach (Unit unit in PlayerTeam.Units)
                unit.GetComponent<ClickReporter>().OnReporterClickedEvent -= OnUnitClicked;

            CleanUpOfferedEffects();
            _effectOptions.CleanUpEffectOptions();
            gameObject.SetActive(false);
        }

        private void EffectSelectedEventHandler(object sender, EventArgs e)
        {
            if (_effectOptions.SelectedEffect.Target == ModificationEffect.TargetType.Team)
                ApplySelectedEffect(null);
            else
                ChooseATargetText.gameObject.SetActive(true);
        }

        private void OnUnitClicked(ClickReporter reporter)
        {
            if (_effectOptions.SelectedEffect == null) return;

            var selectedUnit = reporter.GetComponent<Unit>();
            ApplySelectedEffect(selectedUnit);
        }

        private void ApplySelectedEffect(Unit unit)
        {
            _effectOptions.SelectedEffect.Apply(unit: unit, team: PlayerTeam);

            _actionsRemaining--;
            UpdateActionsRemainingText();

            CleanUpOfferedEffects();
            if (_actionsRemaining <= 0)
                EndModificationRound();
            else
                OfferEffectOptions();
        }

        private void OfferEffectOptions()
        {
            _offeredEffects.AddRange(GetRandomEffects(3));
            foreach (ModificationEffect effect in _offeredEffects) effect.Prepare();

            _effectOptions.DisplayEffectOptions(_offeredEffects);
        }

        private void CleanUpOfferedEffects()
        {
            ChooseATargetText.gameObject.SetActive(false);

            foreach (ModificationEffect effect in _offeredEffects) effect.CleanUp();
            _offeredEffects.Clear();
        }

        private IEnumerable<ModificationEffect> GetRandomEffects(int numberOfEffects)
        {
            return Effects.OrderBy(x => Random.value).Take(Mathf.Min(a: numberOfEffects, b: Effects.Count));
        }

        private void UpdateActionsRemainingText()
        {
            ActionsRemainingText.text = $"actions remaining: {_actionsRemaining.ToString()}";
        }
    }
}
