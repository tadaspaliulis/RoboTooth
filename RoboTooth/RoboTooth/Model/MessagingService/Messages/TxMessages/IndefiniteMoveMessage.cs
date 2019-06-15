using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoboTooth.Model.Control;
namespace RoboTooth.Model.MessagingService.Messages.TxMessages
{
    public enum MoveDirection : byte
    {
        EStop = 0,
        EForward = 1,
        EBackwards = 2,
        ETurnLeft = 3,
        ETurnRight = 4
    }

    /// <summary>
    /// A message that instructs the robot to move forwards, backwards, stop or spin in either direction for an indefinite amount of time
    /// </summary>
    public class IndefiniteMoveMessage : TimedMoveMessage
    {
        public IndefiniteMoveMessage(MoveDirection moveDirection, byte speed, byte actionId = 0) : base(moveDirection, speed, 0, actionId) { } //Time 0 means that the action is executed forever, till it gets cancelled
    }

    public class TimedMoveMessage : RawMessage, IActionInitiationMessage
    {
        public TimedMoveMessage(MoveDirection moveDirection, byte speed, ushort timeInMiliseconds, byte actionId = 0) 
            : base((byte)TxMessageIdsEnum.EMoveControlAction, CreateRawData(moveDirection, speed, timeInMiliseconds, actionId)){ }

        /// <summary>
        /// Creates the raw data to be used by base constructor
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        static protected byte[] CreateRawData(MoveDirection moveDirection, byte speed, ushort timeInMiliseconds, byte actionId)
        {
            var timeBytes = BitConverter.GetBytes(timeInMiliseconds);
            return new byte[] { (byte)moveDirection, speed, timeBytes[0], timeBytes[1], actionId }; //The last 2 bytes are duration in microseconds
        }

        /// <summary>
        /// Used to set/access the action Id directly after the message was already created 
        /// </summary>
        private const int _actionIdByteOffset = sizeof(MoveDirection) + sizeof(byte) + sizeof(ushort);
        public byte ActionId
        {
            get
            {
                return rawData[_actionIdByteOffset];
            }
            set
            {
                rawData[_actionIdByteOffset] = value;
            }
        }
        /// <summary>
        /// Used to access the speed byte after the message was created.
        /// </summary>
        private const int _SpeedByteOffset = sizeof(MoveDirection);
        public byte Speed
        {
            get
            {
                return rawData[_SpeedByteOffset];
            }
        }
        
        /// <summary>
        /// Used to access the direction byte after the message was created.
        /// </summary>
        private const int _moveDirectionByteOffset = 0;

        public MoveDirection Direction
        {
            get
            {
                return (MoveDirection)rawData[0];
            }
        }
    }
}
