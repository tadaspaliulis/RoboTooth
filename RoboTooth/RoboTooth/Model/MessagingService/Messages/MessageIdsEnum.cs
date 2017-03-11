using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.MessagingService.Messages
{
    /// <summary>
    /// Ids for received messages
    /// </summary>
    internal enum RxMessageIdsEnum : byte
    {
        EEchoDistance = 0,
        EMagnetometerOrientation = 1,
    }

    /// <summary>
    /// Ids for transmitted messages
    /// </summary>
    internal enum TxMessageIdsEnum : byte
    {
        EMovementMessage,
    }
}
