using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RoboTooth.Model
{
    public enum ConnecStatusEnum
    {
        ENotConnected,
        EAttemptingConnection,
        EConnected,
        EConnectionLost
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
