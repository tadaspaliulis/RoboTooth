using RoboTooth.Model.MessagingService.Messages.RxMessages;
using RoboTooth.Model.MessagingService.Messages.TxMessages;
using RoboTooth.Model.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.Control
{
    public class MotorsController : IMotorState, IMovementController
    {
        public MotorsController(MessagingService.MessagingService messagingService, MotorSimulator motorSimulator)
        {
            _messagingService = messagingService;
            _motorSimulator = motorSimulator;
        }

        #region Movement commands

        public void StopMovement()
        {
            performRobotMovementAction(new IndefiniteMoveMessage(MoveDirection.EStop, 0));
        }

        public byte TurnClockwise(Duration duration, float speed)
        {
            return performRobotMovementAction(CreateTimedMoveMessage(MoveDirection.ETurnRight, speed, duration));
        }

        public byte TurnCounterClockwise(Duration duration, float speed)
        {
            return performRobotMovementAction(CreateTimedMoveMessage(MoveDirection.ETurnLeft, speed, duration));
        }

        public byte MoveForwardTimed(Duration duration, float speed)
        {
            return performRobotMovementAction(CreateTimedMoveMessage(MoveDirection.EForward, speed, duration));
        }

        public byte MoveBackwardsTimed(Duration duration, float speed)
        {
            return performRobotMovementAction(CreateTimedMoveMessage(MoveDirection.ETurnLeft, speed, duration));
        }

        #endregion

        public void HandleActionCompletedMessage(object sender, ActionCompletedMessage message)
        {
            Console.WriteLine("Completed an action.");
            //Figure which queue this action belonged to and then pop it
            if (message.GetQueueId() == _motorActionQueue.GetQueueId())
            {
                Console.WriteLine("Queue id matched.");
                var actionId = message.GetActionId();
                _motorActionQueue.PopCompletedAction(actionId);

                UpdateInternalMotorState();

                //Could be an opportunity for the controller to react to this action being completed here
                MovementActionCompleted?.Invoke(this, actionId);
            }
        }

        public event EventHandler<byte> MovementActionCompleted;

        #region IMotorState implementations

        public float GetCurrentSpeedPercentage()
        {
            return _currentMotorSpeedPercentage;
        }

        public MotorState GetCurrentDirection()
        {
            return _currentState;
        }

        #endregion

        #region Private methods

        private static TimedMoveMessage CreateTimedMoveMessage(MoveDirection directon, float speed, Duration duration)
        {
            return new TimedMoveMessage(directon,
                                        ConvertSpeedFromPercentageToHw(speed),
                                        (ushort)duration.Miliseconds);
        }

        private byte performRobotMovementAction(TimedMoveMessage action)
        {
            // AddActionToQueue needs to be called before MotorSimulator::AddCommand
            // so that the ActionId field would get set.
            _motorActionQueue.AddActionToQueue(action);
            _messagingService.SendMessage(action);

            // Add the movement/rotation action to the simulation.
            _motorSimulator.AddCommand(Duration.CreateFromMiliSeconds((long)action.DurationInMiliSeconds),
                                      ConvertMoveDirectionToMotorState(action.Direction),
                                      ConvertSpeedFromHwToPercentage(action.Speed),
                                      action.ActionId);

            UpdateInternalMotorState();

            return action.ActionId;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        private static byte ConvertSpeedFromPercentageToHw(float speed)
        {
            return (byte)(255.0f * speed);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        private static float ConvertSpeedFromHwToPercentage(byte speed)
        {
            return (float)speed / 255.0f;
        }

        private static MotorState ConvertMoveDirectionToMotorState(MoveDirection direction)
        {
            switch (direction)
            {
                case MoveDirection.EStop:
                    return MotorState.Idle;
                case MoveDirection.EForward:
                    return MotorState.MoveForward;
                case MoveDirection.EBackwards:
                    return MotorState.MoveBackwards;
                case MoveDirection.ETurnLeft:
                    return MotorState.RotateCounterClockwise;
                case MoveDirection.ETurnRight:
                    return MotorState.RotateClockwise;
            }

            throw new ArgumentException("Unexpected move direction: " + direction);
        }

        /// <summary>
        /// Updates the internal tracking of the motor state.
        /// </summary>
        private void UpdateInternalMotorState()
        {
            var currentAction = _motorActionQueue.GetCurrentAction();
            if (currentAction == null)
            {
                _currentState = MotorState.Idle;
                _currentMotorSpeedPercentage = 0.0f;
                return;
            }

            var timedMoveMessage = currentAction as TimedMoveMessage;
            if(timedMoveMessage == null)
            {
                throw new InvalidOperationException("Wrong Message type in the motor action queue:" + _motorActionQueue.GetCurrentAction().GetType().ToString());
            }

            //This presumably needs a lock around it..
            _currentMotorSpeedPercentage = ConvertSpeedFromHwToPercentage(timedMoveMessage.Speed);
            _currentState = ConvertMoveDirectionToMotorState(timedMoveMessage.Direction);
            Console.WriteLine(string.Format("Updated MotorState: {0} at speed: {1}", _currentState, _currentMotorSpeedPercentage));
        }
        
        #endregion

        private MotorState _currentState = MotorState.Idle;
        private float _currentMotorSpeedPercentage = 0.0f;

        private MessagingService.MessagingService _messagingService;
        private MotorSimulator _motorSimulator;
        private RobotActionQueue _motorActionQueue = new RobotActionQueue(0);
    }
}
