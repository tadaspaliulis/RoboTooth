using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoboTooth.Model.MessagingService;
using RoboTooth.Model.MessagingService.Messages;
using RoboTooth.Model.MessagingService.Messages.RxMessages;
using RoboTooth.Model.MessagingService.Messages.TxMessages;

namespace RoboTooth.Model.Control
{
    public class RoboController
    {
        public RoboController(MessagingService.MessagingService messagingService, MessageSorter messageSorter)
        {
            _messagingService = messagingService;

            _messageSorter = messageSorter;
            _messageSorter.EchoDistanceMessages.MessageReceived += handleEchoDistanceMessage;
            _messageSorter.MagnetometerOrientationMessages.MessageReceived += handleMagnetometerOrientationMessage;
        }

        private void handleEchoDistanceMessage(object sender, EchoDistanceMessage message) { }
        private void handleMagnetometerOrientationMessage(object sender, MagnetometerOrientationMessage message) { }


        public void TurnLeft()
        {
            _messagingService.SendMessage(new IndefiniteMoveMessage(IndefiniteMoveMessage.MoveDirection.ETurnLeft, 255));
        }

        public void TurnRight()
        {
            _messagingService.SendMessage(new IndefiniteMoveMessage(IndefiniteMoveMessage.MoveDirection.ETurnRight, 255));
        }

        public void MoveForward()
        {
            _messagingService.SendMessage(new IndefiniteMoveMessage(IndefiniteMoveMessage.MoveDirection.EForward, 255));
        }

        public void MoveBackwards()
        {
            _messagingService.SendMessage(new IndefiniteMoveMessage(IndefiniteMoveMessage.MoveDirection.EBackwards, 255));
        }

        public void StopMovement()
        {
            _messagingService.SendMessage(new IndefiniteMoveMessage(IndefiniteMoveMessage.MoveDirection.EStop, 0));
        }

        private MessagingService.MessagingService _messagingService;
        private MessageSorter _messageSorter;
    }
}
