using System;
using System.IO;

namespace RoboTooth.Model
{
    public enum ConnecStatusEnum
    {
        NotConnected,
        AttemptingConnection,
        DeviceNotFound,
        PlatformNotAvailable,
        Connected,
        ConnectionLost
    }

    public class ConnectionEvent
    {
        public ConnecStatusEnum ConnectionStatus;
    }

    /// <summary> Interface for low level data communications with the Robot </summary>
    public interface ICommunicationInterface
    {
        bool IsConnected { get; }

        void EstablishConnection();

        Stream GetConnectionStream();

        event EventHandler<ConnectionEvent> ConnectionEvent;
    }
}
