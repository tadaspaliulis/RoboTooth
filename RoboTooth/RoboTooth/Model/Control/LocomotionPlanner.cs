using RoboTooth.Model.Kinematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.Control
{
    public class MovementCommand
    {
        public MovementCommand(Vector2 movementVector, float speed)
        {
            Move = movementVector;
            Speed = speed;
        }

        /// <summary>
        /// Percentage of the maximum robot movement speed.
        /// </summary>
        public readonly float Speed;

        /// <summary>
        /// New location in absolute coordinates.
        /// </summary>
        public readonly Vector2 Move;
    }

    /// <summary>
    /// Concerned with translating change in position/orientation
    /// vectors into robot commands. Monitors the appropriate response messages
    /// and matches that up against the KinematicsModel simulation, makes adjustments if necessary.
    /// 
    /// Informs the callers on the progress of carried out commands.
    /// </summary>
    public class LocomotionPlanner
    {
        public LocomotionPlanner(ISolver solver, IPositionState positionState, IMotorState motorState, IMovementController motorsController, float allowedDistanceError, Angle allowedOrientationError)
        {
            _solver = solver;
            _positionState = positionState;
            _movementController = motorsController;
            _motorState = motorState;

            _allowedDistanceError = allowedDistanceError;
            _allowedOrientationError = allowedOrientationError;

            _targetOrientation = positionState.GetCurrentOrientation();
            _targetPosition = positionState.GetCurrentPosition();

            //TODO: Figure out if this should still listen to kinematics simulation events.
            //Subscribe to Kinematics Model updates.
            //_positionState.CurrentOrientationUpdated += OnOrientationChangedEvent;
            //_positionState.CurrentPositionUpdated += OnPositionChangedEvent;

            _motorState.MovementActionCompleted += OnMovementActionCompleted;
        }

        #region Public methods

        public void ChangePosition(MovementCommand movement)
        {
            //Rotate first, if necessary.
            var deltaDistance = movement.Move - _positionState.GetCurrentPosition();
            var requiredOrientationForMovement = Vector2.Normalize(deltaDistance);

            //Again, absolute v relative vector mismatch
            _targetPosition = movement.Move;
            _targetOrientation = requiredOrientationForMovement;
            _targetSpeedPercentage = movement.Speed;

            bool isRotationNeeded = StartRotationIfNecessary(requiredOrientationForMovement, movement.Speed);

            //If the robot is already facing the right way, just start moving in that direction.
            if(!isRotationNeeded)
            {
                StartMovement(movement.Move, movement.Speed);
            }
        }

        #endregion

        #region Private methods

        private bool StartRotationIfNecessary(Vector2 desiredOrientation, float speedPercentage)
        {
            bool rotateClockwise;
            var currentOrientation = _positionState.GetCurrentOrientation();
            var rotationDuration = _solver.CalculateRotationDurationForNewOrientation(currentOrientation, desiredOrientation, speedPercentage, out rotateClockwise);

            //This should be something more lenient than just epsilon, maybe a milisecond? Probably even that is too much
            //Don't think the motors will spin up in just a milisecond.
            //Should figure out what units we're actually dealing with here!
            if (Math.Abs(rotationDuration.Seconds) > float.Epsilon)
            {
                Console.WriteLine(string.Format("About to start rotation for {0} ms", rotationDuration.Miliseconds));
                Console.WriteLine($"Current orientation is ({currentOrientation.X},{currentOrientation.Y})");
                Console.WriteLine($"Desired orientation is ({desiredOrientation.X},{desiredOrientation.Y})");
                //TODO: FIGURE OUT ROTATION DIRECTION STUFF HERE
                if (rotateClockwise)
                {
                   _orientationActionId = _movementController.TurnClockwise(rotationDuration, speedPercentage);
                }
                else
                {
                   _orientationActionId = _movementController.TurnCounterClockwise(rotationDuration, speedPercentage);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetLocation">Absolute location</param>
        /// <param name="speedPercentage"></param>
        private void StartMovement(Vector2 targetLocation, float speedPercentage)
        {
            _targetPosition = targetLocation;
            _targetSpeedPercentage = speedPercentage;

            var deltaDistance = targetLocation - _positionState.GetCurrentPosition();
            var movementDuration = _solver.CalculateMovementDurationForDeltaDistance(deltaDistance, speedPercentage);
            Console.WriteLine($"Starting movement with dV({deltaDistance.X},{deltaDistance.Y} for {movementDuration.Seconds} seconds.");
            _movementActionId = _movementController.MoveForwardTimed(movementDuration, speedPercentage);
        }

        private bool IsOrientationWithinMargin(Angle angleBetweenExpectedActual, Angle allowedOrientationError)
        {
            //Could the angle be negative here?
            return angleBetweenExpectedActual.Radians < allowedOrientationError.Radians;
        }

        private bool IsPositionWithinMargin(float distance, float allowedDistanceError)
        {
            //Could distance be negative here?
            return distance < allowedDistanceError;
        }

        private void HandleMovementActionComplete()
        {
            var currentPosition = _positionState.GetCurrentPosition();
            var distance = (_targetPosition - currentPosition).Length();
            var isPositionWithinMargin = IsPositionWithinMargin(distance, _allowedDistanceError);

            if (isPositionWithinMargin)
            {
                Console.WriteLine($"Position within margins. Current position ({currentPosition.X},{currentPosition.Y})."
                                    + $" Distance from expected is  {distance}");

                //Inform listeners that we've reached the destination.
                MovementCommandComplete?.Invoke(this, EventArgs.Empty);
  
            }
            else
            {
                //TODO: Need to write adjustment code.
                Console.WriteLine($"Position outside margins with difference of {distance}. Current position ({currentPosition.X},{currentPosition.Y}).");
                //StartMovement(_targetPosition, _targetSpeedPercentage /** 0.5f*/); //Trying again
                ChangePosition(new MovementCommand(_targetPosition, _targetSpeedPercentage));
            }
        }

        private void HandleOrientationActionComplete()
        {
            var currentOrientation = _positionState.GetCurrentOrientation();
            var angleDifference = Trigonometry.CalculateAngle(_targetOrientation, currentOrientation);

            if (IsOrientationWithinMargin(angleDifference, _allowedOrientationError))
            {
                //Now proceed with the movement to target position.
                Console.WriteLine($"Starting movement after orientation with {angleDifference.Degrees} degrees of error.");
                Console.WriteLine($"Current orientation is ({currentOrientation.X},{currentOrientation.Y})");
                StartMovement(_targetPosition, _targetSpeedPercentage);
            }
            else
            {
                //Try rotating again, but this time at half the requested speed.
                Console.WriteLine($"Retrying rotation because angle different was {angleDifference.Degrees} and" +
                                  $" exceeded allowed error of {_allowedOrientationError.Degrees} degrees.");
                StartRotationIfNecessary(_targetOrientation, /*0.5f **/ _targetSpeedPercentage);
            }
        }

        /// <summary>
        /// Handler for motor actions being completes.
        /// Adjusts the location as necessary or reports back that if target position is reached.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="actionId">The ID of the action that was completed.</param>
        private void OnMovementActionCompleted(object sender, byte actionId)
        {
            if(_movementActionId.HasValue && _movementActionId.Value == actionId)
            {
                _movementActionId = null;
                HandleMovementActionComplete();
            }
            else if (_orientationActionId.HasValue && _orientationActionId.Value == actionId)
            {
                _orientationActionId = null;
                HandleOrientationActionComplete();
            }
            else
            {
                throw new ArgumentException($"Received an unexpected action id: {actionId}");
            }
        }

        private bool AreMotorsActive()
        {
            return _movementController.GetCurrentSpeedPercentage() < float.Epsilon;
        }

        #endregion

        public event EventHandler MovementCommandComplete;

        #region Private variables

        private readonly float _allowedDistanceError;
        private readonly Angle _allowedOrientationError;

        private IPositionState _positionState;
        private IMovementController _movementController;
        private IMotorState _motorState;
        private ISolver _solver;

        /// <summary>
        /// Target motor speed to be used for various motions.
        /// 
        /// A lower value might be used, but never higher.
        /// </summary>
        private float _targetSpeedPercentage;


        private Vector2 _targetOrientation;
        private byte? _orientationActionId;

        private Vector2 _targetPosition;
        private byte? _movementActionId;


        #endregion
    }
}
