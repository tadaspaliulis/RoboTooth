using System.Numerics;

namespace RoboTooth.Model.State
{
    public class MovementRecord
    {
        public readonly ushort RelativeTime;
        public readonly Vector2 StartPosition;

        private bool _isPlannedOnly;

        /// <summary>
        /// Indicates whether the action has been executed 
        /// or is only planned to be executed in the future.
        /// </summary>
        public bool IsPlannedOnly { get => _isPlannedOnly; set => _isPlannedOnly = value; }

        /// <summary>
        /// End point of the movement.
        /// </summary>
        public Vector2 Destination;

        public MovementRecord(ushort relativeTime, Vector2 startPosition, Vector2 destination)
        {
            RelativeTime = relativeTime;
            StartPosition = startPosition;
            Destination = destination;
            _isPlannedOnly = true;
        }
    }
}
