using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoboTooth.Model;

namespace RoboTooth.ViewModel
{
    public class ConnectionManagementView : ObservableObject
    {
        public ConnectionManagementView(ICommunicationInterface comms)
        {
            _comms = comms;
            TextStatus = "Not connection yet.";
            _isConnected = false;
            _currentConnectionStatus = ConnecStatusEnum.ENotConnected;
            //Hook up connection events to the Comms
            comms.ConnectionEvent += ConnectionEventHandler;

            //Initialise connection button
            var connectionCommand = new AsyncCommand(new Func<object, bool>(CanExecuteConnectButton), (object a) => _comms.EstablishConnection());

            ConnectionEventOccured += connectionCommand.StateChangeHandler; //External event monitor for potential can execute chhanges

            ConnectionButton = new ObservableButton(connectionCommand, null);
            ConnectionButton.Content = "Connect";
        }

        private bool CanExecuteConnectButton(object a)
        {
            return _currentConnectionStatus == ConnecStatusEnum.ENotConnected;
        }

        private void ConnectionEventHandler(object sender, ConnectionEvent e)
        {
            _currentConnectionStatus = e.ConnectionStatus;
            IsConnected = e.ConnectionStatus == ConnecStatusEnum.EConnected;
            switch(e.ConnectionStatus)
            {
                case ConnecStatusEnum.EConnected:
                    TextStatus = "Connected to Robot.";
                    break;
                case ConnecStatusEnum.EAttemptingConnection:
                    TextStatus = "Attempting connection";
                    break;
                default:
                    TextStatus = "Default case. Wot Happened?";
                    break;
            }

            //Simply relay the connection event command, might wanna change this? Seems a bit pointless just reinvoking it
            //Especially with less data
            ConnectionEventOccured.Invoke(this, new EventArgs());
        }

        private ConnecStatusEnum _currentConnectionStatus;
        private ICommunicationInterface _comms;

        public event EventHandler ConnectionEventOccured;

        public bool _isConnected;
        public bool IsConnected
        {
            private set
            {
                _isConnected = value;
                NotifyPropertyChanged();
            }
            get
            {
                return _isConnected;
            }
        }

        private string _textStatus;
        public string TextStatus
        {
            get
            {
                return _textStatus;
            }
            set
            {
                _textStatus = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableButton _connectionButton;
        public ObservableButton ConnectionButton
        {
            get
            {
                return _connectionButton;
            }
            set
            {
                _connectionButton = value;
                NotifyPropertyChanged();
            }
        }
    }
}
