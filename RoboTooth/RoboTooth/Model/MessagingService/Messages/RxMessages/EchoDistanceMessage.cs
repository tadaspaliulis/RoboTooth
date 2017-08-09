using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.MessagingService.Messages.RxMessages
{
    public class EchoDistanceMessage : RawMessage
    {
        public EchoDistanceMessage(byte[] rawData) : base( (byte)RxMessageIdsEnum.EEchoDistance, rawData )
        {

        }

        public EchoDistanceMessage(RawMessage other) : base(other)
        {

        }

        /// <summary>
        /// Gets the distance carried by the message
        /// </summary>
        /// <returns>Echo Distance sensor distance value</returns>
        public float GetDistance()
        {
            //var dataCopy = new byte[rawData.Length];
            //rawData.CopyTo(dataCopy, 0);
            //Array.Reverse(dataCopy);
            return BitConverter.ToSingle(rawData, 0);
        }

    }
}
