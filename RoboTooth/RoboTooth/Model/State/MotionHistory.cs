using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace RoboTooth.Model.State
{
    /// <summary>
    /// Tracks the motions of the robot throughout its operation.
    /// </summary>
    public class MotionHistory
    {
        public event Action<MovementRecord> NewMovementRecordAdded;

        public MotionHistory()
        {
            Reset();
        }

        public void AddNewMovement(MovementRecord movementRecord)
        {
            _history.Add(movementRecord);

            if (NewMovementRecordAdded != null)
            {
                NewMovementRecordAdded(movementRecord);
            }
        }

        /// <summary>
        /// Clears the motion history.
        /// </summary>
        private void Reset()
        {
            _history.Clear();
        }

        private List<MovementRecord> _history = new List<MovementRecord>();
    }
}
