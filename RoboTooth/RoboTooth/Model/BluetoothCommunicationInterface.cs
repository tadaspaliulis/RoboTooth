using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using InTheHand.Net.Bluetooth;
using System.IO;
using InTheHand.Net.Sockets;
using InTheHand.Net;
using System.Management;

namespace RoboTooth.Model
{

    public class BluetoothCommunicationInterface : ICommunicationInterface
    {
        public event EventHandler<ConnectionEvent> ConnectionEvent;

        public BluetoothCommunicationInterface()
        {
            discoveredDevices = new List<BluetoothDeviceInfo>();
        }

        #region Public methods

        public void EstablishConnection()
        {
            OnConnectionEvent(new ConnectionEvent
            {
                ConnectionStatus = ConnecStatusEnum.EAttemptingConnection
            });

            //Need to dynamically determine MAC address here
            byte[] address = { 0xCF, 0x06, 0xEA, 0xDD, 0xC2, 0x28, 0x00, 0x00 };
            BluetoothAddress macAddress = new BluetoothAddress(address);            
            BluetoothEndPoint endPoint = new BluetoothEndPoint(macAddress, BluetoothService.BluetoothBase/*new Guid("00001101-0000-1000-8000-00805F9B34FB")*//*BluetoothService.SerialPort*/);
            _bluetoothClient = new BluetoothClient();
            _localComponent = new BluetoothComponent(_bluetoothClient);

            ScanForConnections();
        }

        public Stream GetConnectionStream()
        {
            return _bluetoothClient.GetStream();
        }

        public bool IsConnected
        {
            get
            {
                //TODO: Needs to be fixed!
                return true;
            }
        }

        #endregion

        #region Private methods

        private void ScanForConnections()
        {
            _localComponent.DiscoverDevicesProgress += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesProgress);
            _localComponent.DiscoverDevicesComplete += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesComplete);
            _localComponent.DiscoverDevicesAsync(255, true, true, true, true, null);
        }

        /// <summary>
        /// The pin used to authenticate the connection.
        /// </summary>
        private const string RoboPin = "1234";

        private void ConnectToDevice(BluetoothDeviceInfo device)
        {
            // Check if device is paired
            if (!device.Authenticated)
            {
                // Set pin of device to connect with.
                _bluetoothClient.SetPin(RoboPin);
            }

            // Initiate the connection.
            _bluetoothClient.BeginConnect(device.DeviceAddress, BluetoothService.SerialPort, new AsyncCallback(connectionCallback), device);
        }

        private void RecordDiscoveredDevices(DiscoverDevicesEventArgs e)
        {
            for (int i = 0; i < e.Devices.Length; i++)
            {
                discoveredDevices.Add(e.Devices[i]);
            }
        }

        //Event handler for devices being found
        private void component_DiscoverDevicesProgress(object sender, DiscoverDevicesEventArgs e)
        {
            //Does this method need to do anything?
            //RecordDiscoveredDevices(e);
        }

        private void component_DiscoverDevicesComplete(object sender, DiscoverDevicesEventArgs e)
        {
            RecordDiscoveredDevices(e);

            // log some stuff
            foreach (var device in discoveredDevices)
            {
                Console.WriteLine("Discovered device: " + device.DeviceName + " (" + device.DeviceAddress + ")");
                if(device.DeviceName.Equals("RoboTooth"))
                {
                    ConnectToDevice(device);
                    return;
                }
            }
        }

        // callback
        private void connectionCallback(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                OnConnectionEvent(new ConnectionEvent
                {
                    ConnectionStatus = ConnecStatusEnum.EConnected,
                });
                Console.WriteLine("Successfully connected to RoboTooth.");
            }
        }

        private void OnConnectionEvent(ConnectionEvent e)
        {
            var handler = ConnectionEvent;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Private variables

        private BluetoothClient _bluetoothClient;
        private BluetoothComponent _localComponent;

        private List<BluetoothDeviceInfo> discoveredDevices;

        #endregion
    }
}
