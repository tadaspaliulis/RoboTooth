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
        public MotionHistory()
        {
            ResetToStartingPoint();
        }

        /// <summary>
        /// Sets the history to its starting point.
        /// A.K.A. a zero vector.
        /// </summary>
        private void ResetToStartingPoint()
        {
            _history.Clear();

            //Create our starting point.
            AddNewPoint(0, 0, 0, 1, 0);
        }

        public void AddNewPoint(ushort relativeTime, float x, float y, float orientationX, float orientationY)
        {
            _history.Add(new PositionSnapshot(relativeTime, x, y, orientationX, orientationY));
        }

        private List<PositionSnapshot> _history = new List<PositionSnapshot>();
    }
}
