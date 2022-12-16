using System;
using UnityEngine;

namespace GulchGuardians.Coordination
{
    [CreateAssetMenu(fileName = "GameState", menuName = "State/Game State")]
    public class GameState : ScriptableObject
    {
        [field: NonSerialized] public NightPhase NightPhase { get; private set; }
        [field: NonSerialized] public BattlePhase BattlePhase { get; private set; }
        [field: NonSerialized] public int Night { get; private set; }

        public void SetNightPhase(NightPhase phase)
        {
            NightPhase = phase;
            if (phase == NightPhase.Battle) SetBattlePhase(BattlePhase.Preparation);
        }

        public void IncrementNight() { Night++; }

        public void SetBattlePhase(BattlePhase phase) { BattlePhase = phase; }
    }
}
