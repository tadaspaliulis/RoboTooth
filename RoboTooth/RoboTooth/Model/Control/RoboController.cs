using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using RoboTooth.Model.Kinematics;
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
        public RoboController(MessagingService.MessagingService messagingService, MessageSorter messageSorter, MotionHistory motionHistory)
        {
            _messagingService = messagingService;

            //Subscribe to various message events from the message sorter.
            _messageSorter = messageSorter;
            _messageSorter.EchoDistanceMessages.MessageReceived += handleEchoDistanceMessage;
            _messageSorter.MagnetometerOrientationMessages.MessageReceived += handleMagnetometerOrientationMessage;
            _messageSorter.DebugStringMessages.MessageReceived += handleDebugStringMessage;

            _motorsController = new MotorsController(_messagingService);
            _messageSorter.ActionCompletedMessages.MessageReceived += _motorsController.HandleActionCompletedMessage;

            _motionHistory = motionHistory;
            _kinematicsModel = new KinematicsModel(10, 10);
            _navigationPlanner = new NavigationPlanner(_kinematicsModel, _motorsController, _motionHistory);
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

        private void handleDebugStringMessage(object sender, DebugStringMessage message)
        {
            System.Diagnostics.Debug.WriteLine(message.GetDebugString());
        }

        #endregion

        #region Robot Actions

        private void performRobotMovementAction(TimedMoveMessage action)
        {
            //_motorActionQueue.AddActionToQueue(action);
            //_messagingService.SendMessage(action);
        }

        public void TurnLeftIndefinite()
        {
            performRobotMovementAction(new IndefiniteMoveMessage(MoveDirection.ETurnLeft, 255));
        }

        public void TurnRightIndefinite()
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnRight, 255, 5000));
        }

        public void MoveForwardIndefinite()
        {
            performRobotMovementAction(new IndefiniteMoveMessage(MoveDirection.EForward, 255));
           _navigationPlanner.Test();
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

        private MotorsController _motorsController;

        private MotionHistory _motionHistory;

        private KinematicsModel _kinematicsModel;

        private NavigationPlanner _navigationPlanner;
    }
}
