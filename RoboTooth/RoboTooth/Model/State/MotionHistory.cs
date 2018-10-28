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
        public event Action<MovementRecord> NewMotionRecordAdded;

        public MotionHistory()
        {
            Reset();
        }

        public void AddNewPoint(ushort relativeTime, float x, float y, float orientationX, float orientationY)
        {
            _history.Add(new MovementRecord(relativeTime, x, y, orientationX, orientationY));
        }

        public Vector2 GetCurrentPosition()
        {
            return _currentPosition;
        }

        public Vector2 GetCurrentOrientation()
        {
            return _currentOrientation;
        }

        /// <summary>
        /// Sets the history to its starting point.
        /// A.K.A. a zero vector.
        /// </summary>
        private void Reset()
        {
            _history.Clear();

            _currentPosition = Vector2.Zero;
            _currentOrientation = Vector2.UnitY;
        }

        private Vector2 _currentPosition;
        private Vector2 _currentOrientation;

        private List<MovementRecord> _history = new List<MovementRecord>();
    }
}
