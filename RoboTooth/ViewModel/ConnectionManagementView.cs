﻿using RoboTooth.Model;
using RoboTooth.ViewModel.Commands;
using RoboTooth.ViewModel.Commands.CanExecuteEvalTriggers;
using System;

namespace RoboTooth.ViewModel
{
    public class ConnectionManagementView : ObservableObject
    {
        #region Private variables

        private readonly ICommunicationInterface _comms;
        private bool _isConnectionInProgress = false;

        #endregion

        public event EventHandler ConnectionEventOccured;

        public ConnectionManagementView(ICommunicationInterface comms)
        {
            _comms = comms;
            TextStatus = "No connection yet.";
            _isConnected = false;
            //Hook up connection events to the Comms
            comms.ConnectionEvent += ConnectionEventHandler;

            //Initialise connection button
            var connectionCommand = new AsyncCommand(CanExecuteConnectButton, (object a) => _comms.EstablishConnection());
            var canExecuteTrigger = new EventOccurredCanExecuteTrigger();
            connectionCommand.AddCanExecuteChangedTrigger(canExecuteTrigger);

            //External event monitor for potential can execute changes
            ConnectionEventOccured += canExecuteTrigger.HandleEventReceived;

            ConnectionButton = new ObservableButton(connectionCommand, null);
            ConnectionButton.Content = "Connect";
        }

        private bool CanExecuteConnectButton(object a)
        {
            return !IsConnected && !_isConnectionInProgress;
        }

        private void ConnectionEventHandler(object sender, ConnectionEvent e)
        {
            switch (e.ConnectionStatus)
            {
                case ConnecStatusEnum.Connected:
                    IsConnected = true;
                    _isConnectionInProgress = false;
                    TextStatus = "Connected to the Robot.";
                    break;
                case ConnecStatusEnum.AttemptingConnection:
                    IsConnected = false;
                    _isConnectionInProgress = true;
                    TextStatus = "Attempting connection.";
                    break;
                case ConnecStatusEnum.DeviceNotFound:
                    IsConnected = false;
                    _isConnectionInProgress = false;
                    TextStatus = "Could not find the device.";
                    break;
                case ConnecStatusEnum.PlatformNotAvailable:
                    IsConnected = false;
                    _isConnectionInProgress = false;
                    TextStatus = "Connection stack not available.";
                    break;
                default:
                    IsConnected = false;
                    _isConnectionInProgress = false;
                    TextStatus = "Default case. Wot Happened?";
                    break;
            }

            //Simply relay the connection event command, might wanna change this? Seems a bit pointless just reinvoking it
            //Especially with less data
            ConnectionEventOccured.Invoke(this, new EventArgs());
        }

        #region Properties

        public bool _isConnected;
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            private set
            {
                _isConnected = value;
                NotifyPropertyChanged();
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

        #endregion
    }
}
