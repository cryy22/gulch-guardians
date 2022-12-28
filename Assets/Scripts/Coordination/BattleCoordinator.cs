using System;
using System.Collections;
using System.Collections.Generic;
using Crysc.Coordination;
using Crysc.Helpers;
using GulchGuardians.Audio;
using GulchGuardians.Squads;
using GulchGuardians.Teams;
using GulchGuardians.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GulchGuardians.Coordination
{
    public class BattleCoordinator : Coordinator
    {
        [SerializeField] private GameState State;

        [SerializeField] private SquadConfig PlayerSquadConfig;
        [SerializeField] private List<Platoon> EnemyPlatoons;
        [SerializeField] private SquadFactory SquadFactory;

        [SerializeField] private PreparationCoordinator PreparationCoordinator;
        [SerializeField] private CombatCoordinator CombatCoordinator;
        [SerializeField] private Transform PlayerTeamContainer;
        [SerializeField] private Team PlayerTeam;
        [SerializeField] private Team EnemyTeam;

        [SerializeField] private Button AdvanceButton;
        [SerializeField] private UIGamePhaseAnnouncer GamePhaseAnnouncer;

        private static readonly Vector2 _squadMaxSize = new(x: 7, y: 0);
        private static readonly Vector2 _squadSpacingRatio = new(x: -0.125f, y: 0);
        private TMP_Text _advanceButtonText;

        public int EnemyPlatoonCount => EnemyPlatoons.Count;

        protected override void Awake()
        {
            base.Awake();
            _advanceButtonText = AdvanceButton.GetComponentInChildren<TMP_Text>();
        }

        private void Start() { AdvanceButton.onClick.AddListener(OnAdvanceButtonClicked); }

        public override void BeginCoordination()
        {
            base.BeginCoordination();

            PopulateEnemyTeam();
            StartCoroutine(OnboardPlayerTeam());
            StartCoroutine(EnterPreparationPhase());
        }

        public void OnAdvanceInput(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (State.BattlePhase != BattlePhase.Preparation || PreparationCoordinator.IsActive) return;
            OnAdvance();
        }

        public void PopulatePlayerTeam()
        {
            Squad playerSquad = SquadFactory.Create(PlayerSquadConfig);
            PlayerTeam.AddSquad(playerSquad);
        }

        private void PopulateEnemyTeam()
        {
            if (State.Night >= EnemyPlatoons.Count) return;

            Platoon enemyPlatoon = EnemyPlatoons[State.Night];
            foreach (SquadConfig squadConfig in enemyPlatoon.SquadConfigs)
            {
                Squad squad = SquadFactory.Create(squadConfig);
                EnemyTeam.AddSquad(squad);
            }
        }

        private void OnAdvanceButtonClicked()
        {
            if (State.BattlePhase != BattlePhase.Preparation) return;
            OnAdvance();
        }

        private void OnAdvance() { StartCoroutine(RunCombatPhase()); }

        private IEnumerator RunCombatPhase()
        {
            State.SetBattlePhase(BattlePhase.Transition);
            yield return PreparationCoordinator.EndModificationRound();

            BGMPlayer.Instance.TransitionToCombat();
            yield return GamePhaseAnnouncer.AnnouncePhase(isPreparation: false);

            _advanceButtonText.text = "next";
            AdvanceButton.interactable = false;

            State.SetBattlePhase(BattlePhase.Combat);
            yield return CombatCoordinator.RunCombat();

            if (PlayerTeam.IsDefeated || EnemyTeam.IsDefeated) EndCoordination();
            else StartCoroutine(EnterPreparationPhase());
        }

        private IEnumerator EnterPreparationPhase()
        {
            State.SetBattlePhase(BattlePhase.Transition);

            BGMPlayer.Instance.TransitionToPreparation();
            yield return GamePhaseAnnouncer.AnnouncePhase(isPreparation: true);

            _advanceButtonText.text = "fight!";
            AdvanceButton.interactable = true;

            PreparationCoordinator.BeginCoordination();
            State.SetBattlePhase(BattlePhase.Preparation);
        }

        private IEnumerator OnboardPlayerTeam()
        {
            PlayerTeam.transform.SetParent(PlayerTeamContainer);

            PlayerTeam.View.FrontSquadMaxSize = _squadMaxSize;
            PlayerTeam.View.IsCentered = false;
            PlayerTeam.FrontSquad.View.PreferredSpacingRatio = _squadSpacingRatio;

            List<Coroutine> coroutines = new()
            {
                StartCoroutine(Mover.MoveTo(transform: PlayerTeam.transform, end: Vector3.zero, duration: 0.5f)),
                StartCoroutine(PlayerTeam.View.AnimateRearrange(0.5f)),
            };

            yield return CoroutineWaiter.RunConcurrently(coroutines.ToArray());
        }

        [Serializable]
        private struct Platoon
        {
            public List<SquadConfig> SquadConfigs;
        }
    }
}
