using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GulchGuardians.Abilities;
using GulchGuardians.Squads;
using GulchGuardians.Teams;
using GulchGuardians.Units;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GulchGuardians.Coordinators
{
    public class CombatCoordinator : MonoBehaviour
    {
        [SerializeField] private GameState State;
        [SerializeField] private Team PlayerTeam;
        [SerializeField] private Team EnemyTeam;

        [SerializeField] private AbilityType EvasiveType;
        [SerializeField] private AbilityType HealerType;
        [SerializeField] private Button AdvanceButton;
        [SerializeField] private Button AutoButton;
        [SerializeField] private TMP_Text TrySpacebarText;

        private bool _didPlayerAdvance;
        private bool _isAutoAdvance;
        private bool _hasUsedSpacebar;
        private TMP_Text _autoButtonText;

        private void Awake() { _autoButtonText = AutoButton.GetComponentInChildren<TMP_Text>(); }

        private void Start()
        {
            AdvanceButton.onClick.AddListener(OnAdvanceButtonClicked);
            AutoButton.onClick.AddListener(OnAutoButtonClicked);
        }

        public IEnumerator RunCombat()
        {
            AutoButton.gameObject.SetActive(true);
            TrySpacebarText.gameObject.SetActive(!_hasUsedSpacebar && !_isAutoAdvance);

            PlayerTeam.ResetUnitsOnDeck();
            EnemyTeam.ResetUnitsOnDeck();

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
            TrySpacebarText.gameObject.SetActive(false);
        }

        public void OnAdvanceInput(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (State.Phase != Phase.Combat) return;

            OnAdvance();

            if (_hasUsedSpacebar) return;
            _hasUsedSpacebar = true;
            TrySpacebarText.gameObject.SetActive(false);
        }

        private void OnAdvanceButtonClicked()
        {
            if (State.Phase != Phase.Combat) return;
            OnAdvance();
        }

        private void OnAdvance() { _didPlayerAdvance = true; }

        private void OnAutoButtonClicked()
        {
            _isAutoAdvance = !_isAutoAdvance;
            _autoButtonText.text = _isAutoAdvance ? "Auto: On" : "Auto: Off";
            TrySpacebarText.gameObject.SetActive(!_isAutoAdvance && !_hasUsedSpacebar);
        }

        private IEnumerator RunSquadAttack(Squad attackingSquad, Squad defendingSquad)
        {
            IEnumerable<Unit> actors = attackingSquad.Units.Where((u, index) => u.WillAct(index));
            foreach (Unit actor in actors)
            {
                yield return RotateEvasiveAwayFromFront(defendingSquad);

                var attackContext = new AttackContext(
                    actor: actor,
                    defender: defendingSquad.FrontUnit,
                    attackingSquad: attackingSquad,
                    defendingSquad: defendingSquad
                );
                yield return RunUnitAttack(attackContext);

                if (defendingSquad.Count <= 0) yield break;
            }
        }

        private IEnumerator RunUnitAttack(AttackContext context)
        {
            Unit actor = context.Actor;
            Unit defender = context.Defender;

            bool actorIsHealer = actor.HasAbility(HealerType);

            if (actorIsHealer) yield return actor.HealSquad();
            else yield return actor.AttackUnit(target: defender);

            yield return WaitForPlayer();

            if (Unit.IsDefeated(defender))
            {
                yield return context.DefendingSquad.HandleUnitDefeat(defender);
                yield break;
            }

            if (context.DefendingSquad.UnitsRotate && !actorIsHealer)
                yield return RotateSquad(squad: context.DefendingSquad, withHurtAnimation: true);
        }

        private IEnumerator RotateEvasiveAwayFromFront(Squad squad)
        {
            int evasiveCount = squad.Units.Count(u => u.HasAbility(EvasiveType));
            if (evasiveCount == 0 || evasiveCount == squad.Count) yield break;

            while (squad.FrontUnit.HasAbility(EvasiveType))
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
