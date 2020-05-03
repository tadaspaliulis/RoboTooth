using System.Numerics;

namespace RoboTooth.Model.Kinematics
{
    /// <summary>
    /// Responsible for calculating the various 'physics' problems
    /// needed to simulate the robot in a physical world. 
    /// Can be affected by the external stimuli as input from various sensors comes in
    /// and calibrates the the calculations.
    /// 
    /// Does not maintain the state of the robot.
    /// </summary>
    public class SolverNaive : ISolver
    {
        public SolverNaive(float maximumMovementSpeed, float maximumRotationSpeed)
        {
            _maximumMovementSpeed = maximumMovementSpeed;
            _maximumRotationSpeed = maximumRotationSpeed;
        }

        public float GetCurrentRotationSpeed(float speedPercentage)
        {
            return _maximumRotationSpeed * speedPercentage;
        }

        public float GetCurrentMovementSpeed(float speedPercentage)
        {
            return _maximumMovementSpeed * speedPercentage;
        }

        public Vector2 CalculateMovementVector(Vector2 orientation, float motorSpeedPercent, float duration)
        {
            var distance = motorSpeedPercent * duration;
            return orientation * distance;
        }

        /// <summary>
        /// TODO: Write Description
        /// </summary>
        /// <param name="deltaDistance"></param>
        /// <param name="motorSpeedPercentage"></param>
        /// <returns></returns>
        public Duration CalculateMovementDurationForDeltaDistance(Vector2 deltaDistance, float motorSpeedPercentage)
        {
            //Assuming perfect model for now. Will need to be updated with odometry later on.
            var totalDistance = deltaDistance.Length();

            return Duration.CreateFromSeconds(totalDistance / (_maximumMovementSpeed * motorSpeedPercentage));
        }

        /// <summary>
        /// TODO: Write Description
        /// </summary>
        /// <param name="newOrientation"></param>
        /// <param name="motoSpeedPercentage"></param>
        /// <param name="rotateClockwise"></param>
        /// <returns></returns>
        public Duration CalculateRotationDurationForNewOrientation(Vector2 initialOrientation, Vector2 newOrientation, float motoSpeedPercentage, out bool rotateClockwise)
        {
            var angleBetweenVectors = Trigonometry.CalculateDirectionalAngle(initialOrientation, newOrientation);
            rotateClockwise = angleBetweenVectors.IsClockwise;
            return Duration.CreateFromSeconds((float)angleBetweenVectors.Degrees / (motoSpeedPercentage * _maximumRotationSpeed));
        }

        public Vector2 CalculateMovementVector(Vector2 orientation, float motorSpeedPercent, Duration duration)
        {
            //TODO: This will need calibration etc.
            return orientation * duration.Seconds * motorSpeedPercent * _maximumMovementSpeed;
        }

        public DirectionalAngle CalculateDeltaRotation(float speedPercentage, Duration deltaTime, bool clockwise)
        {
            return DirectionalAngle.CreateFromDegrees(deltaTime.Seconds * speedPercentage * _maximumRotationSpeed, clockwise);
        }

        #region Private variables

        private readonly float _maximumMovementSpeed;
        private readonly float _maximumRotationSpeed;

        #endregion
    }
}
