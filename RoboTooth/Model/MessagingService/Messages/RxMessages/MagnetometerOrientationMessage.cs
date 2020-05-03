using System;

namespace RoboTooth.Model.MessagingService.Messages.RxMessages
{
    public class MagnetometerOrientationMessage : RawMessage
    {
        public MagnetometerOrientationMessage(byte[] rawData) : base((byte)RxMessageIdsEnum.EMagnetometerOrientation, rawData)
        {

        }

        public MagnetometerOrientationMessage(RawMessage other) : base(other)
        {

        }

        public short GetX()
        {
            /*var dataCopy = new byte[sizeof(short)];
            Array.Copy(rawData, 0, dataCopy, 0, sizeof(short));
            Array.Reverse(dataCopy);
            return BitConverter.ToInt16(dataCopy, 0);*/
            return BitConverter.ToInt16(rawData, 0);
        }

        public short GetY()
        {
            /*var dataCopy = new byte[sizeof(short)];
            Array.Copy(rawData, sizeof(short), dataCopy, 0, sizeof(short));
            Array.Reverse(dataCopy);
            return BitConverter.ToInt16(dataCopy, 0);*/
            return BitConverter.ToInt16(rawData, sizeof(short));
        }

        public short GetZ()
        {
            /*var dataCopy = new byte[sizeof(short)];
            Array.Copy(rawData, sizeof(short) * 2, dataCopy, 0, sizeof(short));
            Array.Reverse(dataCopy);
            return BitConverter.ToInt16(dataCopy, 0);*/
            return BitConverter.ToInt16(rawData, sizeof(short) * 2);
        }
    }
}
