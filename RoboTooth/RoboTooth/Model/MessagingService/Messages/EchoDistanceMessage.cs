using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.MessagingService.Messages
{
    public class EchoDistanceMessage : RawMessage
    {
        public EchoDistanceMessage(byte[] rawData) : base( (byte)RxMessageIdsEnum.EEchoDistance, rawData )
        {

        }

        public EchoDistanceMessage(RawMessage other) : base(other)
        {

        }
    }
}
