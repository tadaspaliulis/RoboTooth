using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.MessagingService.Messages
{
    public class RawMessage
    {
        public RawMessage(byte Id, byte[] rawData)
        {
            this.Id = Id;
            this.rawData = rawData;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">RawMessage which will have its data copied to this one</param>
        public RawMessage(RawMessage other)
        {
            this.Id = other.Id;

            //Should this be a deep copy instead?
            this.rawData = other.rawData;
        }

        public int GetRawDataLength()
        {
            return rawData.Length;
        }

        public byte[] ToByteArray()
        {
            int byteArrayLength = rawData.Length + MessageHeaderLength;
            byte[] byteArray = new byte[byteArrayLength];

            Array.Copy(rawData, 0, byteArray, MessageHeaderLength, byteArray.Length);

            //Message Framing
            byteArray[0] = byteArray[1] = StartOfFrame;
            byteArray[2] = (byte)rawData.Length;
            byteArray[3] = Id;

            return byteArray;
        }

        public byte Id;
        protected byte[] rawData;

        public string GetRawDataAsString()
        {
            if (rawData.Length < 1)
                return string.Empty;

            string s = string.Empty;
            for (int i = 0; i < (rawData.Length - 1); ++i)
            {
                s += string.Format("{0:X2}", rawData[i]) + " ";
            }
            s += string.Format("{0:X2}", rawData[rawData.Length - 1]);
            
            return s;
        }

        //2 framing bytes at the start, 1 byte for data length, 1 byte for msgId and finally length of data itself
        public static int MessageHeaderLength = 2 + 1 + 1;

        public static byte StartOfFrame = 0xbb;
    }
}
