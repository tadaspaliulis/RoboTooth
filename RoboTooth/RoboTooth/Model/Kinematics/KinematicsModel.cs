using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using RoboTooth.Model.Control;

namespace RoboTooth.Model.Kinematics
{
    /// <summary>
    /// Estimates the motion and resultant change vectors of the robot.
    /// 
    /// Initial implementation will have to assume 'perfect' estimation.
    /// </summary>
    public class KinematicsModel : IPositionState
    {
        #region Private variables

        private Vector2 _currentPosition;
        private Vector2 _currentOrientation;

        private IMotorState _motorState;
        private ISolver _solver;

        #endregion

        #region Events

        public event Action<Vector2> CurrentPositionUpdated;

        public event Action<Vector2> CurrentOrientationUpdated;

        #endregion

        public KinematicsModel(IMotorState motorState, ISolver solver)
        {
            _solver = solver;
            _motorState = motorState;

            _currentPosition = Vector2.Zero;
            _currentOrientation = Vector2.UnitY;
        }

        #region Private methods

        private void UpdateCurrentPosition(Vector2 newPosition)
        {
            _currentPosition = newPosition;
            CurrentPositionUpdated?.Invoke(newPosition);
        }

        private void UpdateCurrentOrientation(Vector2 newOrientation)
        {
            _currentOrientation = newOrientation;
            CurrentOrientationUpdated?.Invoke(newOrientation);
        }

        /// <summary>
        /// Checks whether the motors are running.
        /// </summary>
        /// <returns>True if at least one of the mortors is running</returns>
        private bool AreMotorsActive()
        {
            return _motorState.GetCurrentSpeedPercentage() > float.Epsilon;
        }

        private float GetCurrentRotationSpeed()
        {
            return _solver.GetCurrentRotationSpeed(_motorState.GetCurrentSpeedPercentage());
        }

        private float GetCurrentMovementSpeed()
        {
            return _solver.GetCurrentMovementSpeed(_motorState.GetCurrentSpeedPercentage());
        }

        private Vector2 CalculateDeltaMoveForward(float speedPercentage, Duration deltaTime)
        {
            return _solver.CalculateMovementVector(_currentOrientation, speedPercentage, deltaTime);
        }

        private Vector2 CalculateDeltaMoveBackward(float speedPercentage, Duration deltaTime)
        {
            return _solver.CalculateMovementVector(-(_currentOrientation), speedPercentage, deltaTime);
        }

        private DirectionalAngle CalculateOrientationChange(float speedPercentage, Duration deltaTime, bool clockwise)
        {
            return _solver.CalculateDeltaRotation(speedPercentage, deltaTime, clockwise);
        }

        #endregion

        private object _simulationLock = new object();

        /// <summary>
        /// Should run this continuously and update based on time passed/correct using odometry
        /// and other sensor readings.
        /// </summary>
        /// <param name="deltaTime">Time passed since the last update (units?)</param>
        public void Simulate(Duration deltaTime)
        {
            lock (_simulationLock)
            {
                if (!AreMotorsActive())
                    return;

                var currentSpeed = _motorState.GetCurrentSpeedPercentage();

                //At the moment there's no banking (curved motion). It's only straight movements
                //or rotations in place.
                //The initial model will assume no acceleration since it's hard to meassure anyway.
                var degreesOfRotation = DirectionalAngle.CreateFromDegrees(0, false);
                var deltaDistance = Vector2.Zero;
                switch (_motorState.GetCurrentDirection())
                {
                    case MotorState.Idle:
                        //Robot is motionless.
                        return;

                    case MotorState.MoveForward:
                        deltaDistance = CalculateDeltaMoveForward(currentSpeed, deltaTime);//deltaTime * GetCurrentMovementSpeed() * _currentOrientation;
                        break;
                    case MotorState.MoveBackwards:
                        deltaDistance = CalculateDeltaMoveBackward(currentSpeed, deltaTime);
                        break;
                    case MotorState.RotateCounterClockwise:
                        degreesOfRotation = CalculateOrientationChange(currentSpeed, deltaTime, false);
                        break;
                    case MotorState.RotateClockwise:
                        degreesOfRotation = CalculateOrientationChange(currentSpeed, deltaTime, true);
                        break;
                    default:
                        throw new NotSupportedException("Unexpected MoveDirection: " + _motorState.GetCurrentDirection().ToString());
                }

                if(!deltaDistance.Equals(Vector2.Zero))
                {
                    UpdateCurrentPosition(GetCurrentPosition() + deltaDistance);
                }

                if(degreesOfRotation.Degrees > float.Epsilon)
                {
                    var updatedOrientation = Vector2.Normalize(Trigonometry.RotateVectorByAngle(GetCurrentOrientation(), degreesOfRotation));
                    UpdateCurrentOrientation(updatedOrientation);
                }
            }
        }

        /// <summary>
        /// TODO: Need to do something about race conditions.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetCurrentPosition()
        {
            return _currentPosition;
        }

        /// <summary>
        /// TODO: Need to do something about race conditions.
        /// </summary>
        /// <returns>Orientation of the robot as a normal vector.</returns>
        public Vector2 GetCurrentOrientation()
        {
            return _currentOrientation;
        }
    }
}
