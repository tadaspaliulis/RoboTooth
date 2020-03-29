using RoboTooth.Model.Kinematics;
using RoboTooth.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.Control
{
    /// <summary>
    /// TODO: Write a description.
    /// </summary>
    public class NavigationPlanner
    {
        public NavigationPlanner(IPositionState kinematicsModel, LocomotionPlanner locomotionPlanner, MotionHistory motionHistory)
        {
            _kinematicsModel = kinematicsModel;
            _locomotionPlanner = locomotionPlanner;
            _motionHistory = motionHistory;

            locomotionPlanner.MovementCommandComplete += HandleMovementCommandComplete;
        }

        public void Test()
        {
            MoveToPosition(new Vector2(0, 5), 1.0f);
            MoveToPosition(new Vector2(3, 3), 1.0f);
            MoveToPosition(new Vector2(-1, -1), 1.0f);
            MoveToPosition(new Vector2(-1, 0), 1.0f);
            MoveToPosition(new Vector2(2, 5), 1.0f);
            MoveToPosition(new Vector2(-1, -1), 1.0f);
            MoveToPosition(new Vector2(0, 0), 1.0f);

            //MoveToPosition(new Vector2(0, -10), 1.0f);
            //MoveToPosition(new Vector2(0, 0), 1.0f);
        }

        public void MoveToPosition(Vector2 newPosition, float speedPercentage)
        {
            _movementCommandQueue.Enqueue(new MovementCommand(newPosition, speedPercentage));

            ExecuteActions();
        }

        private void ExecuteActions()
        {
            //Make sure there's something to execute.
            if (_movementCommandQueue.Count == 0 || _currentlyExecutedAction != null)
                return;

            _currentlyExecutedAction = _movementCommandQueue.Dequeue();
            _locomotionPlanner.ChangePosition(_currentlyExecutedAction);
        }

        private void HandleMovementCommandComplete(object sender, EventArgs eventArgs)
        {
            //TODO: Does this need a confirmation that we've actually executed the right command?
            _currentlyExecutedAction = null;

            //TODO: This should be a good opportunity to trigger some pathfinding/whatever logic in the future.

            ExecuteActions();
        }

        #region Private variables

        private Queue<MovementCommand> _movementCommandQueue = new Queue<MovementCommand>();
        private MovementCommand _currentlyExecutedAction = null;

        private readonly IPositionState _kinematicsModel;
        private readonly LocomotionPlanner _locomotionPlanner;
        private readonly MotionHistory _motionHistory;

        #endregion
    }
}
