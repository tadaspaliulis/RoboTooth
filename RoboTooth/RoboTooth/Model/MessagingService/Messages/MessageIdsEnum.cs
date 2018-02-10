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
        EActionCompleted = 2,
        EDebugString = 3,
    }

    /// <summary>
    /// Ids for transmitted messages
    /// </summary>
    internal enum TxMessageIdsEnum : byte
    {
        EMoveControlAction = 1,
    }
}
