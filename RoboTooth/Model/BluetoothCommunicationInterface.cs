using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.IO;

namespace RoboTooth.Model
{
    public class BluetoothCommunicationInterface : ICommunicationInterface
    {
        #region Private variables

        private BluetoothClient _bluetoothClient;
        private BluetoothComponent _localComponent;

        private readonly List<BluetoothDeviceInfo> _discoveredDevices;

        /// <summary>
        /// The pin used to authenticate the connection.
        /// </summary>
        private const string RoboPin = "1234";

        private const string DeviceName = "RoboTooth";

        #endregion

        public bool IsConnected { get; private set; } = false;

        public event EventHandler<ConnectionEvent> ConnectionEvent;

        public BluetoothCommunicationInterface()
        {
            _discoveredDevices = new List<BluetoothDeviceInfo>();
        }

        public void EstablishConnection()
        {
            InvokeConnectionEvent(new ConnectionEvent
            {
                ConnectionStatus = ConnecStatusEnum.AttemptingConnection
            });

            try
            {
                InitialiseBluetoothIfNecessary();
            }
            catch (PlatformNotSupportedException)
            {
                InvokeConnectionEvent(new ConnectionEvent
                {
                    ConnectionStatus = ConnecStatusEnum.PlatformNotAvailable
                });

                return;
            }

            ScanForConnections();
        }

        public Stream GetConnectionStream()
        {
            return _bluetoothClient.GetStream();
        }

        #region Private methods

        private void InitialiseBluetoothIfNecessary()
        {
            if (_bluetoothClient == null || _localComponent == null)
            {
                _bluetoothClient = new BluetoothClient();
                _localComponent = new BluetoothComponent(_bluetoothClient);
            }
        }

        private void ScanForConnections()
        {
            _localComponent.DiscoverDevicesComplete += OnDiscoverDevicesCompleted;
            _localComponent.DiscoverDevicesAsync(255, true, true, true, true, null);
        }

        private void OnDiscoverDevicesCompleted(object sender, DiscoverDevicesEventArgs e)
        {
            foreach (var device in e.Devices)
            {
                Console.WriteLine($"Discovered device: {device.DeviceName} ({device.DeviceAddress})");
                if (device.DeviceName.Equals(DeviceName))
                {
                    BeginConnectionAttempt(device);
                    return;
                }
            }

            InvokeConnectionEvent(new ConnectionEvent
            {
                ConnectionStatus = ConnecStatusEnum.DeviceNotFound
            });

            DestroyBluetoothObjects();
        }

        private void BeginConnectionAttempt(BluetoothDeviceInfo device)
        {
            // Check if device is paired
            if (!device.Authenticated)
            {
                // Set pin of device to connect with.
                _bluetoothClient.SetPin(RoboPin);
            }

            // Initiate the connection.
            _bluetoothClient.BeginConnect(device.DeviceAddress, BluetoothService.SerialPort, new AsyncCallback(ConnectionCallback), device);
        }

        private void ConnectionCallback(IAsyncResult result)
        {
            result.AsyncWaitHandle.WaitOne();
            if (result.IsCompleted)
            {
                IsConnected = true;
                InvokeConnectionEvent(new ConnectionEvent
                {
                    ConnectionStatus = ConnecStatusEnum.Connected,
                });
                Console.WriteLine("Successfully connected to RoboTooth.");
            }
        }

        private void DestroyBluetoothObjects()
        {
            _bluetoothClient = null;
            _localComponent = null;
            IsConnected = false;
        }

        private void InvokeConnectionEvent(ConnectionEvent e)
        {
            ConnectionEvent?.Invoke(this, e);
        }

        #endregion
    }
}
