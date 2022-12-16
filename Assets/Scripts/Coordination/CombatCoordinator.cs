using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Coordination;
using GulchGuardians.Abilities;
using GulchGuardians.Squads;
using GulchGuardians.Teams;
using GulchGuardians.Units;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GulchGuardians.Coordination
{
    public class CombatCoordinator : Coordinator
    {
        private const int _archeryRange = 2;

        [SerializeField] private GameState State;
        [SerializeField] private Team PlayerTeam;
        [SerializeField] private Team EnemyTeam;

        [SerializeField] private AbilityIndex AbilityIndex;
        [SerializeField] private Button AdvanceButton;
        [SerializeField] private Button AutoButton;
        [SerializeField] private GameObject TrySpacebarMessage;

        private bool _didPlayerAdvance;
        private bool _isAutoAdvance;
        private bool _hasUsedSpacebar;
        private TMP_Text _autoButtonText;

        private bool IsCorrectPhase => State.NightPhase == NightPhase.Battle && State.BattlePhase == BattlePhase.Combat;

        private void Awake() { _autoButtonText = AutoButton.GetComponentInChildren<TMP_Text>(); }

        private void Start()
        {
            AdvanceButton.onClick.AddListener(OnAdvanceButtonClicked);
            AutoButton.onClick.AddListener(OnAutoButtonClicked);
        }

        public IEnumerator RunCombat()
        {
            AutoButton.gameObject.SetActive(true);
            TrySpacebarMessage.SetActive(!_hasUsedSpacebar && !_isAutoAdvance);

            Squad playerSquad = PlayerTeam.FrontSquad;
            Squad enemySquad = EnemyTeam.FrontSquad;

            yield return WaitForPlayer(1f);

            while (true)
            {
                if (Squad.IsDefeated(playerSquad) || Squad.IsDefeated(enemySquad)) break;
                yield return RunSquadAttack(attackingSquad: playerSquad, defendingSquad: enemySquad);

                if (Squad.IsDefeated(playerSquad) || Squad.IsDefeated(enemySquad)) break;
                yield return RunSquadAttack(attackingSquad: enemySquad, defendingSquad: playerSquad);
            }

            AutoButton.gameObject.SetActive(false);
            TrySpacebarMessage.SetActive(false);
        }

        public void OnAdvanceInput(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (!IsCorrectPhase) return;

            OnAdvance();

            if (_hasUsedSpacebar) return;
            _hasUsedSpacebar = true;
            TrySpacebarMessage.SetActive(false);
        }

        private void OnAdvanceButtonClicked()
        {
            if (!IsCorrectPhase) return;
            OnAdvance();
        }

        private void OnAdvance() { _didPlayerAdvance = true; }

        private void OnAutoButtonClicked()
        {
            _isAutoAdvance = !_isAutoAdvance;
            _autoButtonText.text = _isAutoAdvance ? "Auto: On" : "Auto: Off";
            TrySpacebarMessage.SetActive(!_isAutoAdvance && !_hasUsedSpacebar);
        }

        private IEnumerator RunSquadAttack(Squad attackingSquad, Squad defendingSquad)
        {
            List<Unit> actors = attackingSquad.Units.Where((u, index) => u.WillAct(index)).ToList();
            while (actors.Count > 0)
            {
                Unit actor = actors.First();
                actors.Remove(actor);

                yield return RotateEvasiveAwayFromFront(defendingSquad);

                var context = new ActionContext(
                    actor: actor,
                    attackingSquad: attackingSquad,
                    defendingSquad: defendingSquad
                );
                yield return RunUnitAttack(context);

                if (Squad.IsDefeated(defendingSquad) || Squad.IsDefeated(attackingSquad)) yield break;
            }
        }

        private IEnumerator RunUnitAttack(ActionContext context)
        {
            bool actorIsHealer = context.Actor.HasAbility(AbilityIndex.Healer);
            bool actorIsTrapper = context.Actor.HasAbility(AbilityIndex.Trapper);
            Unit firstDefender = context.DefendingSquad.FrontUnit;

            yield return ExecuteActorAction(context);

            List<Unit> defeatedUnits = context.AttackingSquad.Units
                .Concat(context.DefendingSquad.Units)
                .Where(Unit.IsDefeated)
                .ToList();
            foreach (Unit unit in defeatedUnits)
                yield return unit.Squad.HandleUnitDefeat(unit);

            if (actorIsHealer || actorIsTrapper || Unit.IsDefeated(firstDefender)) yield break;
            yield return RotateSquad(squad: context.DefendingSquad, withHurtAnimation: true);
        }

        private IEnumerator ExecuteActorAction(ActionContext context)
        {
            Unit actor = context.Actor;

            if (actor.HasAbility(AbilityIndex.Healer))
            {
                yield return actor.HealSquad();
                yield break;
            }

            List<Unit> attackers = context.AttackingSquad.Units.ToList();
            List<Unit> defenders = context.DefendingSquad.Units.ToList();
            Unit defender;

            if (actor.HasAbility(AbilityIndex.Archer))
            {
                int archerIndex = attackers.IndexOf(actor);
                int defenderIndex = _archeryRange - 1 - archerIndex;
                defender = defenders.ElementAtOrDefault(defenderIndex);
            }
            else
            {
                defender = context.DefendingSquad.FrontUnit;
            }

            if (defender == null) yield break;

            yield return actor.AttackUnit(defender);
            yield return WaitForPlayer();
        }

        private IEnumerator RotateEvasiveAwayFromFront(Squad squad)
        {
            int evasiveCount = squad.Units.Count(u => u.HasAbility(AbilityIndex.Evasive));
            if (evasiveCount == 0 || evasiveCount == squad.Count) yield break;

            while (squad.FrontUnit.HasAbility(AbilityIndex.Evasive))
                yield return RotateSquad(squad: squad, withHurtAnimation: false);
        }

        private IEnumerator WaitForPlayer(float autoAdvanceDelay = 0f)
        {
            AdvanceButton.interactable = true;
            yield return new WaitUntil(() => _didPlayerAdvance || _isAutoAdvance);
            if (_isAutoAdvance) yield return new WaitForSeconds(autoAdvanceDelay);

            AdvanceButton.interactable = false;
            _didPlayerAdvance = false;
        }

        private IEnumerator RotateSquad(Squad squad, bool withHurtAnimation = false)
        {
            if (squad.Count <= 1) yield break;

            yield return squad.SetUnitIndex(
                unit: squad.FrontUnit,
                index: squad.Count - 1,
                withHurtAnimation: withHurtAnimation
            );
            yield return WaitForPlayer();
        }
    }
}
