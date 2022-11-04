using System;
using System.Collections;
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
        [SerializeField] private int ActionsPerRound = 2;
        [SerializeField] private List<ModificationEffect> Effects = new();

        [SerializeField] private Team PlayerTeam;

        [SerializeField] private TMP_Text ActionsRemainingText;
        [SerializeField] private TMP_Text ChooseATargetText;
        private readonly List<ModificationEffect> _offeredEffects = new();

        private int _actionsRemaining;
        private EffectOptionsDisplayer _effectOptions;

        private bool IsReady { get; set; } = true;

        private void Awake() { _effectOptions = GetComponent<EffectOptionsDisplayer>(); }

        private void OnEnable()
        {
            _effectOptions.EffectSelected += EffectSelectedEventHandler;
            PlayerTeam.UnitClicked += UnitClickedEventHandler;
        }

        private void OnDisable()
        {
            _effectOptions.EffectSelected -= EffectSelectedEventHandler;
            PlayerTeam.UnitClicked -= UnitClickedEventHandler;
        }

        public void BeginModificationRound()
        {
            gameObject.SetActive(true);
            _actionsRemaining = ActionsPerRound;
            UpdateActionsRemainingText();

            OfferEffectOptions();
        }

        public IEnumerator EndModificationRound()
        {
            yield return new WaitUntil(() => IsReady);

            CleanUpOfferedEffects();
            _effectOptions.CleanUpEffectOptions();
            gameObject.SetActive(false);
        }

        private void UnitClickedEventHandler(object sender, Team.UnitClickedEventArgs e)
        {
            if (_effectOptions.SelectedEffect == null) return;
            StartCoroutine(ApplySelectedEffect(e.Unit));
        }

        private void EffectSelectedEventHandler(object sender, EventArgs e)
        {
            if (_effectOptions.SelectedEffect.Target == ModificationEffect.TargetType.Team)
                StartCoroutine(ApplySelectedEffect(null));
            else
                ChooseATargetText.gameObject.SetActive(true);
        }

        private IEnumerator ApplySelectedEffect(Unit unit)
        {
            IsReady = false;

            yield return _effectOptions.SelectedEffect.Apply(unit: unit, team: PlayerTeam);

            _actionsRemaining--;
            UpdateActionsRemainingText();

            CleanUpOfferedEffects();
            if (_actionsRemaining <= 0)
                StartCoroutine(EndModificationRound());
            else
                OfferEffectOptions();

            IsReady = true;
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
            return Effects
                .Where(e => e.CanBeAppliedTo(PlayerTeam))
                .OrderBy(_ => Random.value)
                .Take(Mathf.Min(a: numberOfEffects, b: Effects.Count));
        }

        private void UpdateActionsRemainingText()
        {
            ActionsRemainingText.text = $"actions remaining: {_actionsRemaining.ToString()}";
        }
    }
}
