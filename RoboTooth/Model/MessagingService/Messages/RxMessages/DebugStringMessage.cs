using System.Text;

namespace RoboTooth.Model.MessagingService.Messages.RxMessages
{
    public class DebugStringMessage : RawMessage
    {
        public DebugStringMessage(byte[] rawData) : base((byte)RxMessageIdsEnum.EDebugString, rawData)
        {
        }

        public DebugStringMessage(RawMessage other) : base(other)
        {
        }

        public string GetDebugString()
        {
            return Encoding.UTF8.GetString(rawData);
        }

    }
}
