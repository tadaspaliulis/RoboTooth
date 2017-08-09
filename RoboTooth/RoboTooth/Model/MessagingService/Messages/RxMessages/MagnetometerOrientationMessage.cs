using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.MessagingService.Messages.RxMessages
{
    public class MagnetometerOrientationMessage : RawMessage
    {
        public MagnetometerOrientationMessage( byte[] rawData) : base( (byte)RxMessageIdsEnum.EMagnetometerOrientation, rawData )
        {

        }

        public MagnetometerOrientationMessage(RawMessage other) : base(other)
        {

        }

        public short GetX()
        {
            return BitConverter.ToInt16(rawData, 0);
        }

        public short GetY()
        {
            return BitConverter.ToInt16(rawData, sizeof(short));
        }

        public short GetZ()
        {
            return BitConverter.ToInt16(rawData, sizeof(short) * 2);
        }
    }
}
