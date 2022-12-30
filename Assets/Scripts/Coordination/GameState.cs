using System;
using GulchGuardians.Coordination.Battle;
using GulchGuardians.Coordination.Camp;
using UnityEngine;

namespace GulchGuardians.Coordination
{
    [CreateAssetMenu(fileName = "GameState", menuName = "State/Game State")]
    public class GameState : ScriptableObject
    {
        [field: NonSerialized] public GamePhase GamePhase { get; private set; }
        [field: NonSerialized] public BattlePhase BattlePhase { get; private set; }
        [field: NonSerialized] public CampPhase CampPhase { get; private set; }
        [field: NonSerialized] public int Night { get; private set; }

        public void SetNightPhase(GamePhase phase)
        {
            GamePhase = phase;
            if (phase == GamePhase.Battle) SetBattlePhase(BattlePhase.Preparation);
            if (phase == GamePhase.Camp) SetCampPhase(CampPhase.Promote);
        }

        public void IncrementNight() { Night++; }

        public void SetBattlePhase(BattlePhase phase) { BattlePhase = phase; }
        public void SetCampPhase(CampPhase phase) { CampPhase = phase; }
    }
}
