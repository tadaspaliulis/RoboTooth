using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.Control
{
    /// <summary>
    /// An action that will be executed by the robot.
    /// Will be removed once the robot sends back a message informing the app that the action was complete.
    /// </summary>
    public class RobotAction
    {
        public RobotAction(byte queueId, byte actionId, IActionInitiationMessage moveMessage)
        {
            _queueId = queueId;
            ActionId = actionId;
            ActionMessage = moveMessage;
        }

        private byte _queueId;
        public byte ActionId { get; private set; }
        public IActionInitiationMessage ActionMessage { get; private set; } //Is this the best way of doing this?
    }
}
