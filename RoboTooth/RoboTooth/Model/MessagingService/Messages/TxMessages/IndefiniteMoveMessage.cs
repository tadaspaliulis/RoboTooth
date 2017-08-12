using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.MessagingService.Messages.TxMessages
{
    /// <summary>
    /// A message that instructs the robot to move forwards, backwards, stop or spin in either direction for an indefinite amount of time
    /// </summary>
    public class IndefiniteMoveMessage : RawMessage
    {
        public IndefiniteMoveMessage(MoveDirection moveDirection, byte speed) : base((byte)TxMessageIdsEnum.EMoveControlIndefinite, CreateRawData(moveDirection, speed)){ }

        public enum MoveDirection : byte
        {
            EStop = 0,
            EForward = 1,
            EBackwards = 2,
            ETurnLeft = 3,
            ETurnRight = 4
        }

        /// <summary>
        /// Creates the raw data to be used by base constructor
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        static private byte [] CreateRawData(MoveDirection moveDirection, byte speed)
        {
            return new byte[] { (byte)moveDirection, speed };
        }
    }
}
