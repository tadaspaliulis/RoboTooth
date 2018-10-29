using RoboTooth.Model.MessagingService.Messages.RxMessages;
using RoboTooth.Model.MessagingService.Messages.TxMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.Control
{
    public class MotorsController
    {
        public MotorsController(MessagingService.MessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        private void performRobotMovementAction(TimedMoveMessage action)
        {
            _motorActionQueue.AddActionToQueue(action);
            _messagingService.SendMessage(action);
        }

        public void StopMovement()
        {
            performRobotMovementAction(new IndefiniteMoveMessage(MoveDirection.EStop, 0));
        }

        public void TurnClockwise(ushort duration)
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnRight, 255, duration));
        }

        public void TurnCounterClockwise(ushort duration)
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnLeft, 255, duration));
        }

        public void MoveForwardTimed(ushort durationMicroS)
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnLeft, 255, durationMicroS));
        }

        public void MoveBackwardsTimed(ushort durationMicroS)
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnLeft, 255, durationMicroS));
        }

        public void HandleActionCompletedMessage(object sender, ActionCompletedMessage message)
        {
            //Figure which queue this action belonged to and then pop it
            if (message.GetQueueId() == _motorActionQueue.GetQueueId())
            {
                _motorActionQueue.PopCompletedAction(message.GetActionId());
                //Could be an opportunity for the controller to react to this action being completed here
            }
        }

        private MessagingService.MessagingService _messagingService;
        private RobotActionQueue _motorActionQueue = new RobotActionQueue(0);
    }
}
