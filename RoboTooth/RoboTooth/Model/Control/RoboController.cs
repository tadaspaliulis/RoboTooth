using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoboTooth.Model.MessagingService;
using RoboTooth.Model.MessagingService.Messages.RxMessages;
using RoboTooth.Model.MessagingService.Messages.TxMessages;
using RoboTooth.Model.State;

namespace RoboTooth.Model.Control
{
    /// <summary>
    /// Object responsible for controlling the robot and dealing with the data retrieved from it
    /// </summary>
    public class RoboController
    {
        public RoboController(MessagingService.MessagingService messagingService, MessageSorter messageSorter)
        {
            _messagingService = messagingService;

            _messageSorter = messageSorter;
            _messageSorter.EchoDistanceMessages.MessageReceived += handleEchoDistanceMessage;
            _messageSorter.MagnetometerOrientationMessages.MessageReceived += handleMagnetometerOrientationMessage;
            _messageSorter.ActionCompletedMessages.MessageReceived += handleActionCompletedMessage;
            _messageSorter.DebugStringMessages.MessageReceived += handleDebugStringMessage;
        }
        #region Robot Message handlers
        private void handleEchoDistanceMessage(object sender, EchoDistanceMessage message) { }
        private void handleMagnetometerOrientationMessage(object sender, MagnetometerOrientationMessage message)
        {
            var val = Math.Atan2(message.GetY() -1800, message.GetZ() + 600);
            //if (val < 0)
            //    val = -val;

            val = val * 180 / Math.PI;
            //System.Diagnostics.Debug.WriteLine(val);
        }
        private void handleActionCompletedMessage(object sender, ActionCompletedMessage message)
        {
            //Figure which queue this action belonged to and then pop it
            if(message.GetQueueId() == _motorActionQueue.GetQueueId())
            {
                _motorActionQueue.PopCompletedAction(message.GetActionId());
                //Could be an opportunity for the controller to react to this action being completed here
            }
        }

        private void handleDebugStringMessage(object sender, DebugStringMessage message)
        {
            System.Diagnostics.Debug.WriteLine(message.GetDebugString());
        }

        #endregion

        #region Robot Actions

        private void performRobotMovementAction(TimedMoveMessage action)
        {
            _motorActionQueue.AddActionToQueue(action);
            _messagingService.SendMessage(action);
        }

        public void TurnLeftIndefinite()
        {
            for (int i = 0; i != 10; ++i)
            {
                performRobotMovementAction(new IndefiniteMoveMessage(MoveDirection.ETurnLeft, 255));
                Task.Delay(20);
            }
        }

        public void TurnRightIndefinite()
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnRight, 255, 5000));
        }

        public void MoveForwardIndefinite()
        {
            performRobotMovementAction(new IndefiniteMoveMessage(MoveDirection.EForward, 255));
        }

        public void MoveBackwardsIndefinite()
        {
            performRobotMovementAction(new IndefiniteMoveMessage(MoveDirection.EBackwards, 255));
        }

        public void StopMovement()
        {
            performRobotMovementAction(new IndefiniteMoveMessage(MoveDirection.EStop, 0));
        }

        public void TurnClockwise(short degrees)
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnRight, 255, getRotationTime(degrees)));
        }

        public void TurnCounterClockwise(short degrees)
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnLeft, 255, getRotationTime(degrees)));
        }

        public void MoveForwardTimed(ushort durationMicroS)
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnLeft, 255, durationMicroS));
        }

        public void MoveBackwardsTimed(ushort durationMicroS)
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnLeft, 255, durationMicroS));
        }

        #endregion

        /// <summary>
        /// Calculate how time robot would need to rotate by specified amount
        /// </summary>
        /// <param name="degrees">An angle for turning</param>
        /// <returns>Number of microseconds needed to turn by specified angle</returns>
        private ushort getRotationTime(short degrees)
        {
            return (ushort)(((float)degrees / 360) * _timeToDo360MicroSeconds);
        }

        const float _timeToDo360MicroSeconds = 3600; //Todo, need to figure out what this value actually is

        private MessagingService.MessagingService _messagingService;
        private MessageSorter _messageSorter;

        private RobotActionQueue _motorActionQueue = new RobotActionQueue(0);

        private MotionHistory _motionHistory;
    }
}
