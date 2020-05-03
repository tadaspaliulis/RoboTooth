using System;
using System.Collections.Generic;

namespace RoboTooth.Model.Control
{
    /// <summary>
    /// Tracks actions that the robot still needs to execute.
    /// Removes actions when it receives messages of completion from the robot.
    /// </summary>
    public class RobotActionQueue //Should this just be using an interface?
    {
        public RobotActionQueue(byte queueId, byte initialActionId = 0)
        {
            _nextActionId = initialActionId;
            _queueId = queueId;
        }

        public void AddActionToQueue(IActionInitiationMessage message)
        {
            message.ActionId = _nextActionId;
            _actionQueue.Enqueue(new RobotAction(_queueId, _nextActionId, message));
            ++_nextActionId;
        }

        public void PopCompletedAction(byte ActionId)
        {
            if (_actionQueue.Count == 0)
                throw new InvalidOperationException("Tried to remove an action from the robot queue even though the queue is empty.");

            var action = _actionQueue.Dequeue();

            if (action.ActionId != ActionId)
                throw new InvalidOperationException("Action being removed from the queue is not at the front. Front of queue id: "
                                                        + action.ActionId + " Removed action id: " + ActionId);
        }

        public IActionInitiationMessage GetCurrentAction()
        {
            if (_actionQueue.Count == 0)
                return null;

            return _actionQueue.Peek().ActionMessage;
        }

        public byte GetQueueId() { return _queueId; }

        private readonly Queue<RobotAction> _actionQueue = new Queue<RobotAction>();
        private byte _nextActionId; //Id that will be assigned to the next action
        private readonly byte _queueId; //A way to differentiate between different queues
    }
}
