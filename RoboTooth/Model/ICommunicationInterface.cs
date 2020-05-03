using System;
using System.IO;

namespace RoboTooth.Model
{
    public enum ConnecStatusEnum
    {
        NotConnected,
        AttemptingConnection,
        DeviceNotFound,
        Connected,
        ConnectionLost
    }

    public class ConnectionEvent
    {
        public ConnecStatusEnum ConnectionStatus;
    }

    public interface ICommunicationInterface
    {
        bool IsConnected { get; }

        Stream GetConnectionStream();
        void EstablishConnection();

        event EventHandler<ConnectionEvent> ConnectionEvent;
    }
}
