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
    /// TODO: Should probably split this out into two classes.
    /// </summary>
    public class MotionHistory
    {
        public event Action<MovementRecord> NewMovementRecordAdded;

        public event Action<MovementRecord> LastMovementRecordUpdated;

        public event Action<MovementRecord> NewMovementPlanAdded;

        public MotionHistory()
        {
            Reset();
        }

        public void AddNewMovement(MovementRecord movementRecord)
        {
            _history.Add(movementRecord);

            NewMovementRecordAdded?.Invoke(movementRecord);
        }

        public void OnOrientationChangedEvent(Vector2 orientation)
        {
            //Means that the next we get a position update we'll start a new 'line'.
            orientationChanged = true;
        }

        public void OnPositionChangedEvent(Vector2 position)
        {
            if (IsNewRecordNeeded())
            {
                //If we've changed orientation or the history is empty, new record should be started.
                //TODO: Might  want to pass this in as a constructor parameter instead.
                Vector2 startPosition = Vector2.Zero;

                if (_history.Count != 0)
                    startPosition = _history.Last().Destination;

                //TODO: figure out what to do with the relative time, is it even necessary?
                AddNewMovement(new MovementRecord(0, startPosition, position));
            }
            else
            {
                UpdateLastRecord(position);
            }
        }

        /// <summary>
        /// If we've changed orientation or the history is empty, new record should be started.
        /// </summary>
        /// <returns>True if new record is needed.</returns>
        private bool IsNewRecordNeeded()
        {
            return orientationChanged || _history.Count == 0;
        }

        /// <summary>
        /// Update the last history item with a new destination
        /// and inform listeners about the change.
        /// </summary>
        /// <param name="position">New destination for the last history item.</param>
        private void UpdateLastRecord(Vector2 position)
        {
            if (_history.Count == 0)
                throw new InvalidOperationException("Cannot update last history item when the history is empty");

            var lastHistoryItem = _history.Last();
            lastHistoryItem.Destination = position;

            LastMovementRecordUpdated?.Invoke(lastHistoryItem);
        }

        /// <summary>
        /// Clears the motion history.
        /// </summary>s
        private void Reset()
        {
            _history.Clear();
        }


        private bool orientationChanged = false;

        /// <summary>
        /// Keeps track of the movements actually made (or movements that we believe were made).
        /// </summary>
        private List<MovementRecord> _history = new List<MovementRecord>();

        /// <summary>
        /// Keeps track of the robot plans.
        /// </summary>
        private List<MovementRecord> _plans = new List<MovementRecord>();
    }
}
