using System;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Squads
{
    public class SquadReorderer : MonoBehaviour
    {
        private Squad _squad;

        private void Awake() { _squad = GetComponent<Squad>(); }

        private void OnEnable()
        {
            foreach (Unit unit in _squad.Units) unit.View.Draggable.Ended += DragEndedHandler;
        }

        private void OnDisable()
        {
            foreach (Unit unit in _squad.Units) unit.View.Draggable.Ended += DragEndedHandler;
        }

        private void DragEndedHandler(object sender, EventArgs e) { ReturnToArrangement(); }

        private void ReturnToArrangement() { _squad.View.UpdateArrangement(); }
    }
}
