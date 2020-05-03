namespace RoboTooth.Model.MessagingService.Messages.RxMessages
{
    public class ActionCompletedMessage : RawMessage
    {
        public ActionCompletedMessage(byte[] rawData) : base((byte)RxMessageIdsEnum.EActionCompleted, rawData) { }

        public ActionCompletedMessage(RawMessage other) : base(other)
        {
        }

        public byte GetActionId() { return rawData[1]; }

        public byte GetQueueId() { return rawData[0]; }
    }
}
