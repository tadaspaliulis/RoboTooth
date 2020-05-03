using RoboTooth.Model.Control;
using RoboTooth.Model.MessagingService.Messages;
using RoboTooth.ViewModel.Commands;
using RoboTooth.ViewModel.Commands.CanExecuteEvalTriggers;
using RoboTooth.ViewModel.DataDisplayVM;
using RoboTooth.ViewModel.Drawing;
using RoboTooth.ViewModel.WorldMap;
using System;
using System.Collections.ObjectModel;

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

        private readonly RawMessage _message;
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

            InitialiseButtons(_connectionManagement, _intDataDisplay, _mainController.GetRoboController());

            //Set up the Canvas drawing
            IntialiseCanvasAndDependants();

            _mainController.GetMotionHistory().NewMovementRecordAdded += MovementMap.HandleNewMovementRecordAdded;
            _mainController.GetMotionHistory().LastMovementRecordUpdated += MovementMap.HandleLastMovementRecordUpdated;
        }

        private void InitialiseControllers()
        {
            _mainController = new MainController();
        }

        private void InitialiseButtons(ConnectionManagementView connection, InternalDataDisplay internalData, RoboController roboController)
        {
            MovementSequenceTestButton = CreateConnectionEnabledAsyncButton(connection, (a) => roboController.Test());

            SetMovementTargetButton = CreateConnectionEnabledAsyncButton(connection,
                (a) => roboController.MoveToLocation(internalData.TargetPositionX, internalData.TargetPositionY));
        }

        private static ObservableButton CreateConnectionEnabledAsyncButton(ConnectionManagementView connection, Action<object> buttonAction)
        {
            var command = new AsyncCommand((a) => { return connection.IsConnected; }, buttonAction);
            command.AddCanExecuteChangedTrigger(new PropertyChangedCanExecuteTrigger(nameof(connection.IsConnected), connection));
            return new ObservableButton(command, null);
        }

        /// <summary>
        /// Helper method for initialising the canvas and
        /// various objects that use it.
        /// </summary>
        private void IntialiseCanvasAndDependants()
        {
            _canvas = new CanvasVM();
            _movementMap = new MovementMapVM(_canvas);

            //Temp test.
            var testWall = new Model.Mapping.Wall();
            testWall.FaceNormal = System.Numerics.Vector2.Normalize(new System.Numerics.Vector2(0.5f, 0.5f));
            testWall.LengthLeft = 20.0f;
            testWall.LengthRight = 20.0f;

            testWall.Position = new System.Numerics.Vector2(5.0f, 5.0f);
            var testWallVm = new WallVM(testWall);
            _canvas.AddDrawable(testWallVm);

            _canvas.AddDrawable(new Line
            {
                StartPointX = 1.0f,
                StartPointY = 1.0f,
                EndPointX = 10.0f,
                EndPointY = 10.0f
            });
        }

        private ObservableCollection<MessageListItem> _rawMessageList;
        public ObservableCollection<MessageListItem> RawMessageList
        {
            get
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

        #region Observable Properties
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

        private MovementMapVM _movementMap;
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

        private CanvasVM _canvas;

        public CanvasVM Canvas
        {
            get { return _canvas; }
            set
            {
                _canvas = value;
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

        #region Movement buttons

        private ObservableButton _movementSequenceTestButton;
        public ObservableButton MovementSequenceTestButton
        {
            get
            {
                return _movementSequenceTestButton;
            }
            set
            {
                _movementSequenceTestButton = value;
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

        private ObservableButton _setMovementTargetButton;
        public ObservableButton SetMovementTargetButton
        {
            get
            {
                return _setMovementTargetButton;
            }
            set
            {
                _setMovementTargetButton = value;
                NotifyPropertyChanged();
            }
        }

        #endregion 

        #endregion
    }
}
