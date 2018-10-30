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
    /// 
    /// </summary>
    public class NavigationPlanner
    {
        public NavigationPlanner(KinematicsModel kinematicsModel, MotorsController motorsController, MotionHistory motionHistory)
        {
            _kinematicsModel = kinematicsModel;
            _motorsController = motorsController;
            _motionHistory = motionHistory;
        }

        public void Test()
        {
            MoveToPosition(new Vector2(0, 50), 1.0f);
            MoveToPosition(new Vector2(15, 15), 1.0f);
            MoveToPosition(new Vector2(-100, -100), 1.0f);
            MoveToPosition(new Vector2(-100, 0), 1.0f);
        }

        public void MoveToPosition(Vector2 newPosition, float speedPercentage)
        {
            //Maybe this should be a set target position and set target orientation instead?


            //Figure out if rotation is needed before we can move.
            //Make sure the vector substraction operand order is the right way around.
            var deltaVector = newPosition - _kinematicsModel.GetCurrentPosition();
            var requiredOrientationForMovement = Vector2.Normalize(deltaVector);

            bool rotateClockwise;
            var rotationDuration =_kinematicsModel.CalculateRotationDurationForNewOrientation(requiredOrientationForMovement, speedPercentage, out rotateClockwise);
            
            //This should be something more lenient than just epsilon, maybe a milisecond? Probably even that is too much
            //Don't think the motors will spin up in just a milisecond.
            //Should figure out what units we're actually dealing with here!
            if (Math.Abs(rotationDuration) > float.Epsilon)
            {
                if (rotateClockwise)
                {
                    //_motorsController.TurnClockwise((ushort)rotationDuration);
                }
                else
                {
                    //_motorsController.TurnCounterClockwise((ushort)rotationDuration);
                }

                _motionHistory.AddNewMovement(new MovementRecord(0, _kinematicsModel.GetCurrentPosition(), requiredOrientationForMovement, Vector2.Zero));
                
                _kinematicsModel.UpdateCurrentOrientation(requiredOrientationForMovement);
            }

            //Then move to the new position
            var movementDuration = _kinematicsModel.CalculateMovementDurationForDeltaDistance(deltaVector, speedPercentage);
            //_motorsController.MoveForwardTimed((ushort)movementDuration);
            _motionHistory.AddNewMovement(new MovementRecord(0, _kinematicsModel.GetCurrentPosition(), _kinematicsModel.GetCurrentOrientation(), deltaVector));
            _kinematicsModel.UpdateCurrentPosition(newPosition);
        }

        private readonly KinematicsModel _kinematicsModel;
        private readonly MotorsController _motorsController;
        private readonly MotionHistory _motionHistory;
    }
}
