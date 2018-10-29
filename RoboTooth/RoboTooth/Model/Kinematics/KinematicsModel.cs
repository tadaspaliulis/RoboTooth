using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace RoboTooth.Model.Kinematics
{
    /// <summary>
    /// Estimates the motion and resultant change vectors of the robot.
    /// 
    /// Initial implementation will have to assume 'perfect' estimation.
    /// </summary>
    public class KinematicsModel
    {
        private float _maximumMovementSpeed;
        private float _maximumRotationSpeed;

        private Vector2 _currentPosition;
        private Vector2 _currentOrientation;

        //private float _wheelDiameter;

        #region Events

        public event Action<Vector2> CurrentPositionUpdated;

        public event Action<Vector2> CurrentOrientationUpdated;

        #endregion

        public KinematicsModel(float maximumMovementSpeed, float maximumRotationSpeed)
        {
            _maximumMovementSpeed = maximumMovementSpeed;
            _maximumRotationSpeed = maximumRotationSpeed;

            _currentPosition = Vector2.Zero;
            _currentOrientation = Vector2.UnitY;
        }

        public Vector2 CalculateMovementVector(float motorSpeedPercent, float duration)
        {
            var distance = motorSpeedPercent * duration;
            return GetCurrentOrientation() * distance;
        }

        public float CalculateMovementDurationForDeltaDistance(Vector2 deltaDistance, float motorSpeedPercentage)
        {
            //Assuming perfect model for now. Will need to be updated with odometry later on.
            var totalDistance = deltaDistance.Length();

            return totalDistance / (_maximumMovementSpeed * motorSpeedPercentage);
        }

        public void UpdateCurrentPosition(Vector2 newPosition)
        {
            _currentPosition = newPosition;
            if (CurrentPositionUpdated != null)
                CurrentPositionUpdated(newPosition);
        }

        public void UpdateCurrentOrientation(Vector2 newOrientation)
        {
            _currentOrientation = newOrientation;
            if (CurrentOrientationUpdated != null)
                CurrentOrientationUpdated(newOrientation);
        }

        /// <summary>
        /// Should run this continuously and update based on time passed/correct using odometry
        /// and other sensor readings.
        /// 
        /// Not implemented/used at the moment.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateModel(float deltaTime)
        {
            throw new NotImplementedException();
        }

        public Vector2 GetCurrentPosition()
        {
            return _currentPosition;
        }

        public Vector2 GetCurrentOrientation()
        {
            return _currentOrientation;
        }
    }
}
