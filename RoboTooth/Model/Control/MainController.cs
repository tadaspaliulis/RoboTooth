using RoboTooth.Model.MessagingService;
using RoboTooth.Model.State;

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

            _motionHistory = new MotionHistory();

            _roboController = new RoboController(_messagingService, _messageSorter, _motionHistory);
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

        internal MotionHistory GetMotionHistory()
        {
            return _motionHistory;
        }

        private readonly MessagingService.MessagingService _messagingService;
        private readonly ICommunicationInterface _communicationInterface;
        private readonly MessageSorter _messageSorter;

        private readonly MotionHistory _motionHistory;

        private readonly RoboController _roboController;
    }
}
