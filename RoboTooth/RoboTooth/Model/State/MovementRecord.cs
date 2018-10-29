using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace RoboTooth.Model.State
{
    public class MovementRecord
    {
        public readonly ushort RelativeTime;
        public readonly Vector2 StartPosition;
        public readonly Vector2 Movement;
        public readonly Vector2 Orientation;

        public Vector2 DestinationPoint
        {
            get { return StartPosition + Movement; }
        }

        public MovementRecord(ushort relativeTime, Vector2 startPosition, Vector2 orientation, Vector2 movement)
        {
            RelativeTime = relativeTime;
            StartPosition = startPosition;
            Orientation = orientation;
            Movement = movement;
        }
    }
}
