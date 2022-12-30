using Crysc.Controls;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Squads
{
    public class SquadReorderer : MonoBehaviour
    {
        private Squad _squad;
        private bool _isReordering;

        private void Awake() { _squad = GetComponent<Squad>(); }

        private void OnEnable()
        {
            if (_isReordering) RegisterEventHandlers();
        }

        private void OnDisable()
        {
            if (_isReordering) UnregisterEventHandlers();
        }

        public void BeginReordering()
        {
            RegisterEventHandlers();
            foreach (Unit unit in _squad.Units) unit.View.Draggable.IsActive = true;

            _isReordering = true;
        }

        public void EndReordering()
        {
            _isReordering = false;

            foreach (Unit unit in _squad.Units) unit.View.Draggable.IsActive = false;
            UnregisterEventHandlers();
        }

        private void RegisterEventHandlers()
        {
            foreach (Unit unit in _squad.Units)
            {
                unit.View.Draggable.Began += DragBeganHandler;
                unit.View.Draggable.Ended += DragEndedHandler;
            }
        }

        private void UnregisterEventHandlers()
        {
            foreach (Unit unit in _squad.Units)
            {
                unit.View.Draggable.Began -= DragBeganHandler;
                unit.View.Draggable.Ended -= DragEndedHandler;
            }
        }

        private void DragBeganHandler(object sender, DraggableEventArgs<Unit> e)
        {
            e.Target.View.SetShowDetails(false);
        }

        private void DragEndedHandler(object sender, DraggableEventArgs<Unit> e)
        {
            e.Target.View.SetShowDetails(true);
            ReturnToArrangement();
        }

        private void ReturnToArrangement() { _squad.View.UpdateArrangement(); }
    }
}
