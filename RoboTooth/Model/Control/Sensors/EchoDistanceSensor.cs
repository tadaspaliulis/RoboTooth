using RoboTooth.Model.Control.Filters;
using RoboTooth.Model.Kinematics;
using RoboTooth.Model.MessagingService.Messages.RxMessages;
using System;
using System.Numerics;

namespace RoboTooth.Model.Control.Sensors
{

    public class EchoDistanceMeasurement
    {
        public EchoDistanceMeasurement(Vector2 MeasurementPoint, Vector2 MeasuredDistance)
        {
            this.MeasurementPoint = MeasurementPoint;
            this.MeasuredDistance = MeasuredDistance;
        }

        /// <summary>
        /// The position where the measurement was made.
        /// </summary>
        public readonly Vector2 MeasurementPoint;

        /// <summary>
        /// The echo distance measurement converted into a vector.
        /// </summary>
        public readonly Vector2 MeasuredDistance;
    }

    /// <summary>
    /// Converts raw echo distance sensor data into vectors.
    /// </summary>
    public class EchoDistanceSensor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="offsetFromCenter">TODO</param>
        /// <param name="angleFromFront">TODO</param>
        public EchoDistanceSensor(IPositionState positionState, IFilter<float> filter, Vector2 offsetFromCenter, DirectionalAngle angleFromFront = null)
        {
            _robotPosition = positionState;
            _filter = filter;
            _offsetFromCenter = offsetFromCenter;
            _angleFromFront = angleFromFront;

            // Subscribe to various events
            _filter.AveragedDataAvailable += HandleFilterResultGenerated;
            positionState.CurrentOrientationUpdated += HandleOrientationChange;
            positionState.CurrentPositionUpdated += HandlePositionChange;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="message">The decoded echo distance message</param>
        public void HandleRawSensorDataReceived(object sender, EchoDistanceMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            _filter.HandleNewDataReceived(message.GetDistance());
        }

        public event EventHandler<EchoDistanceMeasurement> NewDistanceDataAvailable;

        #region Private methods

        private void HandleOrientationChange(Vector2 newOrientation)
        {
            // For now simply reset our filter, but in the future we might
            // want to try and get lower quality results instead
            _filter.Reset();
        }

        private void HandlePositionChange(Vector2 newPosition)
        {
            // For now simply reset our filter, but in the future we might
            // want to try and get lower quality results instead
            _filter.Reset();
        }

        private void HandleFilterResultGenerated(object sender, float result)
        {
            var measurementOrientation = _robotPosition.GetCurrentOrientation();
            if (_angleFromFront != null)
            {
                measurementOrientation = Trigonometry.RotateVectorByAngle(_robotPosition.GetCurrentOrientation(), _angleFromFront);
            }

            var measuredDistance = measurementOrientation * result;
            var measurementPosition = _robotPosition.GetCurrentPosition() + _offsetFromCenter;

            NewDistanceDataAvailable?.Invoke(this, new EchoDistanceMeasurement(measurementPosition, measuredDistance));
        }

        #endregion

        #region Private variables

        private readonly IPositionState _robotPosition;

        /// <summary>
        /// Sensors location in relation to the 'center'
        /// of the robot
        /// </summary>
        private Vector2 _offsetFromCenter;

        /// <summary>
        /// Sensors orientation in relation to the front
        /// of the robot
        /// </summary>
        private readonly DirectionalAngle _angleFromFront;

        private readonly IFilter<float> _filter;

        #endregion
    }
}
