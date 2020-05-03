using RoboTooth.Model.Control;

namespace RoboTooth.Model.Simulation
{
    /// <summary>
    /// Estimates robot velocity based on the state of the robot
    /// </summary>
    public class VelocityEstimator : IVelocityEstimate
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="motorState">Motor state interface</param>
        /// <param name="speedCalibrationMovement">Calibration multiplier for movement speed</param>
        /// <param name="speedCalibrationRotation">Calibration multiplier for rotation speed</param>
        public VelocityEstimator(IMotorState motorState,
                                 float speedCalibrationMovement,
                                 float speedCalibrationRotation)
        {
            _motorState = motorState;
            _speedCalibrationMovement = speedCalibrationMovement;
            _speedCalibrationRotation = speedCalibrationRotation;
        }

        /// <summary>
        /// Gets the current movement velocity estimate
        /// </summary>
        /// <returns>Velocity</returns>
        public float GetMovementVelocity()
        {
            return _motorState.GetCurrentSpeedPercentage() * _speedCalibrationMovement;
        }

        /// <summary>
        /// Gets the current rotation velocity estimate.
        /// </summary>
        /// <returns>Angular velocity</returns>
        public float GetRotationVelocity()
        {
            return _motorState.GetCurrentSpeedPercentage() * _speedCalibrationRotation;
        }

        private readonly IMotorState _motorState;
        private readonly float _speedCalibrationMovement;
        private readonly float _speedCalibrationRotation;
    }
}
