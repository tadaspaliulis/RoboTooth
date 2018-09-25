using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Threading;
using RoboTooth.Model.MessagingService.Messages;

namespace RoboTooth.Model.MessagingService
{
    public class MessagingService
    {
        public MessagingService(ICommunicationInterface communicationInterface)
        {
            this.communicationInterface = communicationInterface;
            communicationInterface.ConnectionEvent += ConnectionEventHandler;

            _receivedDataBuffer = new CircularBuffer<byte>(1024); //1kB
            EnableReceiving = true;
        }

        public event EventHandler<RawMessage> MessageReceivedEvent;

        public bool EnableReceiving { get; set; }

        public void ConnectionEventHandler(object sender, ConnectionEvent e)
        {
            if(e.ConnectionStatus == ConnecStatusEnum.EConnected)
                ProcessReceivedData();
        }

        private readonly object _receivedDataLock = new object();
        private void ProcessReceivedData()
        {
            //Only want a single instance of this running at any time
            lock (_receivedDataBuffer)
            {
                Task _dataReceiverTask = new Task(ProcessReceivedDataInternal);
                _dataReceiverTask.Start();
            }
        }

        private void ProcessReceivedDataInternal()
        {
            try
            {
                Stream dataStream = communicationInterface.GetConnectionStream();
                byte[] bytes = new byte[/*dataStream.Length +*/ 100];
                while (EnableReceiving)
                {
                    // Read may return anything from 0 to 10.
                    int numBytesRead = dataStream.Read(bytes, 0, 15);

                    _receivedDataBuffer.Add(bytes, numBytesRead);
                    if (_receivedDataBuffer.getAvailableDataSize() >= RawMessage.MessageHeaderLength + 1)
                        LookForMessages();

                    Thread.Sleep(50);
                }
                //Maybe callback here on completion?
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //Exception handling, is this even necessary?
            }
        }

        private void OnMessageFound(RawMessage message)
        {
            MessageReceivedEvent?.Invoke(this, message);
        }

        /// <summary>
        /// Looks for Raw Message frames and then passes them on for further recognition/processing
        /// </summary>
        private void LookForMessages()
        {
            return;
            var availableData = _receivedDataBuffer.getAvailableDataSize();
            if (availableData < RawMessage.MessageHeaderLength)
            {
                return; //Don't have enough data yet, do nothing
            }

            int startOfFrameIndex = FindStartOfFrame(0);
            if (startOfFrameIndex < 0)//Negative if start of frame not found
                return;

            int readLocation = startOfFrameIndex + 2; //Skip past
            if (readLocation + 1 > _receivedDataBuffer.getAvailableDataSize())
                return;

            byte messageLength = _receivedDataBuffer.Read(readLocation++);

            //Make sure we have received enough data to read the msg id and the full data payload
            if (messageLength + 1 > (_receivedDataBuffer.getAvailableDataSize() - readLocation))
                return;

            if (messageLength == 0)
                Console.WriteLine("Message length zero???");
            byte messageId = _receivedDataBuffer.Read(readLocation++);

            var messageData = _receivedDataBuffer.copy(readLocation, messageLength);
            RawMessage message = new RawMessage(messageId, messageData);

            //Discard the 'read' data
            _receivedDataBuffer.discardData(readLocation + messageLength);

            //Inform subscribers that a message was found
            OnMessageFound(message);
        }

        /// <summary>
        /// Find the 2 byte start of frame indicator in the received data buffer
        /// </summary>
        /// <param name="startPosition">Location indicating where the search will start</param>
        /// <returns>Virtual index pointing at the start of the frame</returns>
        private int FindStartOfFrame(int startPosition)
        {
            if (startPosition > _receivedDataBuffer.getAvailableDataSize() - 1)
                throw new IndexOutOfRangeException("startPosition points outside the _receivedDataBuffer");

            for (int i = startPosition; i < _receivedDataBuffer.getAvailableDataSize() - 1; ++i)
            {
                if (_receivedDataBuffer.Read(i) == RawMessage.StartOfFrame &&
                        _receivedDataBuffer.Read(i + 1) == RawMessage.StartOfFrame)

                    return i;
            }

            return -1;
        }

        private readonly object _sendMessageLock = new object();
        /// <summary>
        /// Locking method for sending messages to the robot
        /// </summary>
        /// <param name="message">Message to be sent to the robot</param>
        public void SendMessage(RawMessage message)
        {
            lock (_sendMessageLock)
            {
                Stream dataStream = communicationInterface.GetConnectionStream();
                var rawMessageData = message.ToByteArray();
                dataStream.Write(rawMessageData, 0, rawMessageData.Length);
            }
        }

        private ICommunicationInterface communicationInterface;

        private CircularBuffer<byte> _receivedDataBuffer;
    }
}
