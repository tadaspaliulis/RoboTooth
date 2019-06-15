using RoboTooth.Model.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoboTooth.Model.MessagingService.Messages;
using System.Collections.ObjectModel;
using RoboTooth.ViewModel.DataDisplayVM;
using RoboTooth.ViewModel.WorldMap;

namespace RoboTooth.ViewModel
{

    public class MessageListItem
    {
        public MessageListItem(RawMessage message)
        {
            _message = message;
        }

        public string RawData
        {
            get
            {
                return _message.GetRawDataAsString();
            }
        }

        public int Id
        {
            get
            {
                return _message.Id;
            }
        }

        private RawMessage _message;
    }

    /// <summary>
    /// ViewOrchestrator controls all the possible views in the application and initialises control objects
    /// </summary>
    class ViewOrchestrator : ObservableObject
    {
        public ViewOrchestrator()
        {   
            InitialiseControllers();

            _connectionManagement = new ConnectionManagementView(_mainController.GetCommunicationInterface());

            _intDataDisplay = new InternalDataDisplay();
            _mainController.GetMessageSorter().EchoDistanceMessages.MessageReceived += IntDataDisplay.HandleEchoDistanceMessage;
            _mainController.GetMessageSorter().MagnetometerOrientationMessages.MessageReceived += IntDataDisplay.HandleMagnetometerOrientationMessage;
            _mainController.GetRoboController().GetPositionState().CurrentPositionUpdated += IntDataDisplay.HandlePositionUpdated;

            _rawMessageList = new ObservableCollection<MessageListItem>();
            _mainController.GetMessageSorter().UnfilteredMessages += HandleReceivedMessages;

            MoveLeftButton = new ObservableButton(new AsyncCommand((a) => { return true; }, (a) => { _mainController.GetRoboController().Test(); }), null);
            MoveRightButton = new ObservableButton(new AsyncCommand((a) => { return true; }, (a) => { }), null);
            MoveForwardButton = new ObservableButton(new AsyncCommand((a) => { return true; }, (a) => { }), null);
            //The two actions below need fixing!
            MoveBackwardsButton = new ObservableButton(new Command((a) => { return true; }, (a) => { }), null);
            MoveStopButton = new ObservableButton(new Command((a) => { return true; }, (a) => { }), null);

            MovementMap.Lines = new ObservableCollection<LineVM>();

            _mainController.GetMotionHistory().NewMovementRecordAdded += MovementMap.HandleNewMovementRecordAdded;
            _mainController.GetMotionHistory().LastMovementRecordUpdated += MovementMap.HandleLastMovementRecordUpdated;
        }
            
        private void InitialiseControllers()
        {
            _mainController = new MainController();
        }

        private ObservableCollection<MessageListItem> _rawMessageList;
        public ObservableCollection<MessageListItem> RawMessageList
        {   get
            {
                return _rawMessageList;
            }
            set
            {
                _rawMessageList = value;
                NotifyPropertyChanged();
            }
        }

        private void HandleReceivedMessages(object sender, RawMessage message)
        {
            App.Current?.Dispatcher.Invoke(delegate
            {
                _rawMessageList.Add(new MessageListItem(message));
            });
        }

        private MainController _mainController;

        private ConnectionManagementView _connectionManagement;
        public ConnectionManagementView ConnectionManagement
        {
            get
            {
                return _connectionManagement;
            }
            set
            {
                _connectionManagement = value;
                NotifyPropertyChanged();
            }
        }

        private MovementMapVM _movementMap = new MovementMapVM();
        public MovementMapVM MovementMap
        {
            get
            {
                return _movementMap;
            }
            set
            {
                _movementMap = value;
                NotifyPropertyChanged();
            }
        }

        #region Movement buttons

        private ObservableButton _moveLeftButton;
        public ObservableButton MoveLeftButton
        {
            get
            {
                return _moveLeftButton;
            }
            set
            {
                _moveLeftButton = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableButton _moveRightButton;
        public ObservableButton MoveRightButton
        {
            get
            {
                return _moveRightButton;
            }
            set
            {
                _moveRightButton = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableButton _moveBackwardsButton;
        public ObservableButton MoveBackwardsButton
        {
            get
            {
                return _moveBackwardsButton;
            }
            set
            {
                _moveBackwardsButton = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableButton _moveForwardButton;
        public ObservableButton MoveForwardButton
        {
            get
            {
                return _moveForwardButton;
            }
            set
            {
                _moveForwardButton = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableButton _moveStopButton;
        public ObservableButton MoveStopButton
        {
            get
            {
                return _moveStopButton;
            }
            set
            {
                _moveStopButton = value;
                NotifyPropertyChanged();
            }
        }

        private InternalDataDisplay _intDataDisplay;
        public InternalDataDisplay IntDataDisplay
        {
            get
            {
                return _intDataDisplay;
            }
            set
            {
                _intDataDisplay = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
    }
}
