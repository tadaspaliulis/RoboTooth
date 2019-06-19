using System;

namespace RoboTooth.Model.Control
{
    public enum MotorState
    {
        Idle,
        MoveForward,
        MoveBackwards,
        RotateClockwise,
        RotateCounterClockwise,
    }

    public interface IMotorState
    {
        float GetCurrentSpeedPercentage();

        MotorState GetCurrentDirection();

        /// <summary>
        /// Invoked whenever a command is completed, the byte parameter indicates
        /// the ID of completed action.
        /// </summary>
        event EventHandler<byte> MovementActionCompleted;
    }
}
