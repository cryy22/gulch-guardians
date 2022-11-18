using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GulchGuardians.ModificationEffects;
using GulchGuardians.Units;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GulchGuardians.Teams
{
    [RequireComponent(typeof(UIEffectOptionsDisplayer))]
    public class TeamModifier : MonoBehaviour
    {
        [SerializeField] private int ActionsPerRound = 2;
        [SerializeField] private List<ModificationEffect> Effects = new();

        [SerializeField] private Team PlayerTeam;
        [SerializeField] private Team EnemyTeam;

        [SerializeField] private TMP_Text ActionsRemainingText;
        [SerializeField] private TMP_Text ChooseATargetText;
        private readonly List<ModificationEffect> _offeredEffects = new();

        private UIEffectOptionsDisplayer _effectOptions;
        private int _actionsRemaining;
        private bool _isModifying;

        private void Awake() { _effectOptions = GetComponent<UIEffectOptionsDisplayer>(); }

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
            yield return new WaitUntil(() => !_isModifying);

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
            switch (_effectOptions.SelectedEffect.Target)
            {
                case ModificationEffect.TargetType.PlayerTeam:
                case ModificationEffect.TargetType.EnemyTeam:
                    StartCoroutine(ApplySelectedEffect(null));
                    break;
                default:
                    ChooseATargetText.gameObject.SetActive(true);
                    break;
            }
        }

        private IEnumerator ApplySelectedEffect(Unit unit)
        {
            _isModifying = true;

            yield return _effectOptions.SelectedEffect.Apply(BuildContext(unit));

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
                .Where(e => e.CanBeAppliedTo(BuildContext()))
                .OrderBy(_ => Random.value)
                .Take(Mathf.Min(a: numberOfEffects, b: Effects.Count));
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
