using System;
using UnityEngine;

namespace GulchGuardians.Coordinators
{
    [CreateAssetMenu(fileName = "GameState", menuName = "State/Game State")]
    public class GameState : ScriptableObject
    {
        [field: NonSerialized] public Phase Phase { get; private set; }

        public void SetPhase(Phase phase) { Phase = phase; }
    }
}
