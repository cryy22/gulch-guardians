using Crysc.Controls;
using GulchGuardians.Units;
using UnityEngine;

namespace GulchGuardians.Squads
{
    public class SquadReorderer : MonoBehaviour
    {
        private Squad _squad;
        private SquadArrangement _arrangement;
        private bool _isReordering;

        private void Awake()
        {
            _squad = GetComponent<Squad>();
            _arrangement = GetComponent<SquadArrangement>();
        }

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
                unit.View.Draggable.Moved += DragMovedHandler;
                unit.View.Draggable.Ended += DragEndedHandler;
            }
        }

        private void UnregisterEventHandlers()
        {
            foreach (Unit unit in _squad.Units)
            {
                unit.View.Draggable.Began -= DragBeganHandler;
                unit.View.Draggable.Moved -= DragMovedHandler;
                unit.View.Draggable.Ended -= DragEndedHandler;
            }
        }

        private void DragBeganHandler(object sender, DraggableEventArgs<Unit> e)
        {
            Unit unit = e.Target;

            unit.View.SetShowDetails(false);
            _arrangement.ExcludeFromRearrange(unit);
        }

        private void DragMovedHandler(object sender, DraggableEventArgs<Unit> e)
        {
            Unit unit = e.Target;
            int currentIndex = _squad.GetUnitIndex(unit);
            int closestIndex = _arrangement.GetClosestIndex(unit.View.transform.localPosition);

            if (currentIndex == closestIndex) return;

            StartCoroutine(_squad.SetUnitIndex(unit: unit, index: closestIndex));
        }

        private void DragEndedHandler(object sender, DraggableEventArgs<Unit> e)
        {
            Unit unit = e.Target;

            _arrangement.IncludeInRearrange(unit);
            unit.View.SetShowDetails(true);
            _arrangement.Rearrange();
        }
    }
}
