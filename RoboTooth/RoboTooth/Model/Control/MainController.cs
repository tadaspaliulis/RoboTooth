using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoboTooth.Model.MessagingService;

namespace RoboTooth.Model.Control
{
    /// <summary>
    /// An object responsible for orchestrating various control classes in the application. Might be a placeholder for now
    /// </summary>
    internal class MainController
    {
        internal MainController()
        {
            _communicationInterface = new BluetoothCommunicationInterface();
            _messagingService = new MessagingService.MessagingService(_communicationInterface);
            _messageSorter = new MessageSorter();
            _messagingService.MessageReceivedEvent += _messageSorter.HandleRawMessage;
            _roboController = new RoboController(_messagingService, _messageSorter);
        }

        internal ICommunicationInterface GetCommunicationInterface()
        {
            return _communicationInterface;
        }

        internal MessagingService.MessagingService GetMessagingService()
        {
            return _messagingService;
        }

        internal MessageSorter GetMessageSorter()
        {
            return _messageSorter;
        }

        internal RoboController GetRoboController()
        {
            return _roboController;
        }

        private MessagingService.MessagingService _messagingService;
        private ICommunicationInterface _communicationInterface;
        private MessageSorter _messageSorter;
        private RoboController _roboController;
    }
}
