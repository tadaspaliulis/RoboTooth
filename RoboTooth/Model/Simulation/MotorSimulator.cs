using RoboTooth.Model.Control;
using System;
using System.Collections.Generic;

namespace RoboTooth.Model.Simulation
{
    /// <summary>
    /// Attempts to estimate the state of the motors.
    /// </summary>
    public class MotorSimulator : IMotorState, ISimulation
    {
        public MotorState GetCurrentDirection()
        {
            if (_actionQueue.Count == 0)
                return MotorState.Idle;

            var currentAction = _actionQueue.Peek();
            if (currentAction.IsActive())
                return currentAction.State;

            return MotorState.Idle;
        }

        public float GetCurrentSpeedPercentage()
        {
            if (_actionQueue.Count == 0)
                return 0.0f;

            var currentAction = _actionQueue.Peek();
            if (currentAction.IsActive())
                return currentAction.RequestedSpeedPercentage;

            return 0.0f;
        }

        public void Simulate(Duration deltaTime)
        {
            var deltaTimeInner = new Duration(deltaTime);
            while (_actionQueue.Count != 0)
            {
                var currentAction = _actionQueue.Peek();
                deltaTimeInner = currentAction.AdvanceTime(deltaTimeInner);
                if (currentAction.IsComplete())
                {
                    _actionQueue.Dequeue();
                    MovementActionCompleted(this, currentAction.ID);
                }

                if (deltaTimeInner.Miliseconds == 0)
                    break;
            }
        }

        public void AddCommand(Duration commandDuration, MotorState state, float motorSpeedPercentage, byte commandId)
        {
            _actionQueue.Enqueue(new MotorAction
            {
                StartsIn = Duration.CreateFromMiliSeconds(100), //TODO: Might want to make this dynamic
                DurationLeft = commandDuration,
                State = state,
                RequestedSpeedPercentage = motorSpeedPercentage,
                ID = commandId
            });
        }

        /// <summary>
        /// Invoked whenever a command is completed, the byte parameter indicates
        /// the ID of completed action.
        /// </summary>
        public event EventHandler<byte> MovementActionCompleted;

        private readonly Queue<MotorAction> _actionQueue = new Queue<MotorAction>();

        private class MotorAction
        {
            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="deltaTime"></param>
            /// <returns>Left over duration after advancing the action, note, might be 0.</returns>
            public Duration AdvanceTime(Duration deltaTime)
            {
                Duration leftOver = null;
                if (deltaTime.Miliseconds >= StartsIn.Miliseconds)
                {
                    StartsIn = Duration.CreateFromMiliSeconds(0);
                    leftOver = deltaTime.Substract(StartsIn);
                }
                else
                {
                    // We're still waiting for the action to start.
                    StartsIn = StartsIn.Substract(deltaTime);
                    return Duration.CreateFromMiliSeconds(0);
                }

                if (leftOver.Miliseconds >= DurationLeft.Miliseconds)
                {
                    leftOver = leftOver.Substract(DurationLeft);
                    DurationLeft = Duration.CreateFromMiliSeconds(0);
                }
                else
                {
                    // We're executing the action.
                    DurationLeft = DurationLeft.Substract(leftOver);
                    return Duration.CreateFromMiliSeconds(0);
                }

                return leftOver;
            }

            public bool IsComplete()
            {
                return StartsIn.Miliseconds == 0 && DurationLeft.Miliseconds == 0;
            }

            public bool IsActive()
            {
                return StartsIn.Miliseconds == 0 && DurationLeft.Miliseconds != 0;
            }

            public byte ID;

            public Duration DurationLeft;
            public Duration StartsIn;

            public MotorState State;
            public float RequestedSpeedPercentage;
        }
    }
}
