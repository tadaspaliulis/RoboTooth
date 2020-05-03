using System;

namespace RoboTooth.Model.MessagingService.Messages.RxMessages
{
    public class RotaryEncodersMessage : RawMessage
    {
        public RotaryEncodersMessage(byte[] rawData) : base((byte)RxMessageIdsEnum.ERotaryEncoders, rawData) { }

        public RotaryEncodersMessage(RawMessage other) : base(other) { }

        public ushort GetLeftWheelCounter()
        {
            return BitConverter.ToUInt16(rawData, 0);
        }

        public ushort GetRightWheelCounter()
        {
            return BitConverter.ToUInt16(rawData, sizeof(ushort));
        }
    }
}
