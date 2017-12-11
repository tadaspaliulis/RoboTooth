using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.Control
{
    /// <summary>
    /// An interface that needs to be implement by messages that would be used in the RobotActionQueue
    /// </summary>
    public interface IActionInitiationMessage
    {
        /// <summary>
        /// ActionId, needs to be set by the RoboApp
        /// Same ActionId will be returned by the Robot once it completes this action
        /// </summary>
        byte ActionId { get; set; }
    }
}
