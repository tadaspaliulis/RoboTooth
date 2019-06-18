using RoboTooth.Model.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var currentAction  = _actionQueue.Peek();
                deltaTimeInner = currentAction.AdvanceTime(deltaTimeInner);
                if (currentAction.IsComplete())
                    _actionQueue.Dequeue();

                if (deltaTimeInner.Miliseconds == 0)
                    break;
            }
        }

        void AddCommand(Duration commandDuration, MotorState state, float motorSpeedPercentage)
        {
            _actionQueue.Enqueue(new MotorAction
            {
                StartsIn = Duration.CreateFromMiliSeconds(100), //TODO: Might want to make this dynamic
                DurationLeft = commandDuration,
                State = state,
                RequestedSpeedPercentage = motorSpeedPercentage
            });
        }

        private Queue<MotorAction> _actionQueue = new Queue<MotorAction>();

        public class MotorAction
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="deltaTime"></param>
            /// <returns>Left over duration after advancing the action, note, might be 0.</returns>
            public Duration AdvanceTime(Duration deltaTime)
            {
                Duration leftOver = null;
                if(deltaTime.Miliseconds >= StartsIn.Miliseconds)
                {
                    StartsIn = Duration.CreateFromMiliSeconds(0);
                    leftOver = deltaTime.Substract(StartsIn);
                }
                else
                {
                    // We're still waiting for the action to start.
                    StartsIn.Substract(deltaTime);
                    return Duration.CreateFromMiliSeconds(0);
                }

                if (leftOver.Miliseconds >= DurationLeft.Miliseconds)
                {
                    StartsIn = Duration.CreateFromMiliSeconds(0);
                    leftOver = leftOver.Substract(DurationLeft);
                }
                else
                {
                    // We're executing the action.
                    DurationLeft.Substract(leftOver);
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

            public Duration DurationLeft;
            public Duration StartsIn;

            public MotorState State;
            public float RequestedSpeedPercentage;
        }
    }
}
