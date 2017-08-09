using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoboTooth.Model.MessagingService.Messages;
using RoboTooth.Model.MessagingService.Messages.RxMessages;

namespace RoboTooth.Model.MessagingService
{
    public abstract class MessageRecogniserBase
    {
        public MessageRecogniserBase(byte Id)
        {
            this.Id = Id;
        }

        public byte Id { get; private set; }
        public abstract RawMessage HandleRecognisedMessage(RawMessage message);
    }

    public class MessageRecogniser<T> : MessageRecogniserBase where T : RawMessage
    {
        public MessageRecogniser(byte Id, Func<RawMessage, T> preprocessor) : base(Id)
        {
            _preprocessor = preprocessor;
        }

        /// <summary>
        /// Converts message to its inheritted variant and passes it on to the event listeners
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Message (potentially) converted to its inheritted variant</returns>
        public override RawMessage HandleRecognisedMessage(RawMessage message)
        {
            var processedMessage = _preprocessor(message);
            MessageReceived?.Invoke(this, processedMessage);
            return processedMessage;
        }

        //Convert from RawMessage to 
        private Func<RawMessage, T> _preprocessor;

        public event EventHandler<T> MessageReceived;
    }

    /// <summary>
    /// Class responsible for taking in raw messages and spitting out identified, messages wrapped in appropriate classes
    /// </summary>
    public class MessageSorter
    {
        public MessageSorter()
        {
            _recognisers = new List<MessageRecogniserBase>();
            _unrecognisedMessageHandler = new MessageRecogniser<RawMessage>(0, (RawMessage m) => { return m; });

            Initialise();
        }
        
        private void Initialise()
        {
            //Initialisation of all the recognisers here
            AddRecogniser(EchoDistanceMessages = new MessageRecogniser<EchoDistanceMessage>((byte)RxMessageIdsEnum.EEchoDistance, 
                (RawMessage m) => { return new EchoDistanceMessage(m); }));
            AddRecogniser(MagnetometerOrientationMessages = new MessageRecogniser<MagnetometerOrientationMessage>((byte)RxMessageIdsEnum.EMagnetometerOrientation,
                (RawMessage m) => { return new MagnetometerOrientationMessage(m); }));
        }

        #region Message recognisers

        public MessageRecogniser<EchoDistanceMessage> EchoDistanceMessages { get; private set; }

        public MessageRecogniser<MagnetometerOrientationMessage> MagnetometerOrientationMessages { get; private set; }

        #endregion

        public void HandleRawMessage(object sender, RawMessage rawMessage)
        {
            var matchingRecogniser = _recognisers.Find((item) => item.Id == rawMessage.Id);

            RawMessage recognisedMessage = null;
            if (matchingRecogniser != null)
            {
                recognisedMessage = matchingRecogniser.HandleRecognisedMessage(rawMessage);
            }
            else
            {
                recognisedMessage = _unrecognisedMessageHandler.HandleRecognisedMessage(rawMessage);
            }

            //An event for listeners interested in all of the messages passing through here
            UnfilteredMessages?.Invoke(this, recognisedMessage);
        }

        private void AddRecogniser(MessageRecogniserBase recogniser)
        {
            var duplicateRecogniser = _recognisers.Find((item) => item.Id == recogniser.Id);
            if (duplicateRecogniser != null)
                throw new ArgumentException("MessageRecogniser with id " + recogniser.Id + " already added to the MessageSorter.", "recogniser");

            _recognisers.Add(recogniser);
        }

        private MessageRecogniser<RawMessage> _unrecognisedMessageHandler;

        /// <summary>
        /// Invoked whenever a message is not recognised by any of the recognisers
        /// </summary>
        public MessageRecogniser<RawMessage> UnrecognisedMessageHandler
        {
            get
            {
                return _unrecognisedMessageHandler;
            }
        }

        /// <summary>
        /// Event for receiving all the messages after they get converted.
        /// </summary>
        public event EventHandler<RawMessage> UnfilteredMessages; 

        private List<MessageRecogniserBase> _recognisers;
    }
}
