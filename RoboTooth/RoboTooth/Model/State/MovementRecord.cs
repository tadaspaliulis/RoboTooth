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
        public readonly Vector2 Position;
        public readonly Vector2 Orientation;

        public MovementRecord(ushort relativeTime, float x, float y, float orientationX, float orientationY)
        {
            RelativeTime = relativeTime;
            Position = new Vector2(x, y);

            //Make sure the vector is normalized.
            Orientation = Vector2.Normalize(new Vector2(orientationX, orientationY));
        }

        public MovementRecord(ushort relativeTime, Vector2 position, Vector2 orientation)
        {
            this.RelativeTime = relativeTime;
            Position = position;
            orientation = position;
        }
    }
}
