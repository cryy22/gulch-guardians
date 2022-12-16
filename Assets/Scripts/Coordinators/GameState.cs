using System;
using UnityEngine;

namespace GulchGuardians.Coordinators
{
    [CreateAssetMenu(fileName = "GameState", menuName = "State/Game State")]
    public class GameState : ScriptableObject
    {
        [field: NonSerialized] public NightPhase NightPhase { get; private set; }
        [field: NonSerialized] public BattlePhase BattlePhase { get; private set; }

        public void SetNightPhase(NightPhase phase) { NightPhase = phase; }
        public void SetBattlePhase(BattlePhase phase) { BattlePhase = phase; }
    }
}
