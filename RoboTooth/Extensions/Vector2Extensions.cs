using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Extensions
{
    /// <summary>
    /// Extensions for the Vector2 struct
    /// </summary>
    static public class Vector2Extensions
    {
        /// <summary>
        /// Vector2 equality comparison, with a margin of error for each
        /// component of the vector.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="delta"></param>
        /// <returns>True if vectors are equal, false otherwise</returns>
        public static bool EqualsWithinDelta(this Vector2 v1, Vector2 v2, float delta)
        {
            return Math.Abs(v1.X - v2.X) < delta && Math.Abs(v1.Y - v2.Y) < delta;
        }

        /// <summary>
        /// Creates a perpendicular vector, rotated counter clockwise to the original.
        /// </summary>
        /// <param name="v1">Vector</param>
        /// <returns>Perpendicular vector</returns>
        public static Vector2 PerpendicularCounterClockWise(this Vector2 v1)
        {
            return new Vector2(-v1.Y, v1.X);
        }
    }
}
