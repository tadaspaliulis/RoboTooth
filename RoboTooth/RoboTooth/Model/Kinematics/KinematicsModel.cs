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

        //private float _wheelDiameter;

        public KinematicsModel(float maximumMovementSpeed, float maximumRotationSpeed)
        {
            _maximumMovementSpeed = maximumMovementSpeed;
            _maximumRotationSpeed = maximumRotationSpeed;
        }

        public Vector2 CalculateMovementVector(Vector2 orientation, float motorSpeedPercent, float duration)
        {
            var distance = motorSpeedPercent * duration;
            return orientation * distance;
        }

        public Vector2 CalculateOrientationAfterRotation(Vector2 orientation, float motorSppedPercent, float duration)
        {
            throw new NotImplementedException();
        }
    }
}
