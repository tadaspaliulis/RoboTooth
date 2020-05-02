using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.Kinematics
{
    /// <summary>
    /// Helper trigonometry functions.
    /// </summary>
    public static class Trigonometry
    {
        #region Constants

        const int VectorOperationSignificantDigits = 10;

        #endregion

        /// <summary>
        /// Calculates an angle between two normal vectors.
        /// </summary>
        /// <param name="v1">Normal vector</param>
        /// <param name="v2">Normal vector</param>
        /// <returns>Angle between two vectors</returns>
        public static Angle CalculateAngle(Vector2 v1, Vector2 v2)
        {
            var cosineBetweenVectors = Vector2.Dot(v1, v2);
            return Angle.CreateFromRadians(Math.Acos((double)cosineBetweenVectors));
        }

        /// <summary>
        /// Calculates an angle between two normal vectors
        /// and finds the direction for the shortest rotation.
        /// </summary>
        /// <param name="v1">Normal vector</param>
        /// <param name="v2">Normal vector</param>
        /// <returns>Angle between the vectors with directional information</returns>
        public static DirectionalAngle CalculateDirectionalAngle(Vector2 v1, Vector2 v2)
        {
            var cosineBetweenVectors = Vector2.Dot(v1, v2);

            float directionInRadians = (float)(Math.Atan2(v2.Y, v2.X) - Math.Atan2(v1.Y, v1.X));

            //Figure out direction somehow. Below is wrong!!
            bool isClockWiseRotation = directionInRadians < 0;

            return DirectionalAngle.CreateFromRadians(Math.Acos((double)cosineBetweenVectors), isClockWiseRotation);
        }

        /// <summary>
        /// Creates a Vector rotated by the given angle. Positive angles rotate the vector counter clockwise,
        /// wheareas negative will rotate it counter clockwise.
        /// 
        /// Each individual component of the vector gets rounded.s
        /// </summary>
        /// <param name="vector">Vector for rotating</param>
        /// <param name="angle">Angle to rotate the vector by</param>
        /// <returns>Rotated vector</returns>
        public static Vector2 RotateVectorByAngle(Vector2 vector, Angle angle)
        {
            var angleInRadians = angle.Radians;
            float x2 = (float)Math.Cos(angleInRadians) * vector.X - (float)Math.Sin(angleInRadians) * vector.Y;
            float y2 = (float)Math.Sin(angleInRadians) * vector.X + (float)Math.Cos(angleInRadians) * vector.Y;

            return BuildVector2Rounded(x2, y2);
        }

        /// <summary>
        /// Creates a Vector rotated by the given directional angle.
        /// 
        /// Each individual component of the vector gets rounded.s
        /// </summary>
        /// <param name="vector">Vector for rotating</param>
        /// <param name="angle">Angle to rotate the vector by, provides direction in which to rotate
        /// </param>
        /// <returns>Rotated vector</returns>
        public static Vector2 RotateVectorByAngle(Vector2 vector, DirectionalAngle angle)
        {
            return RotateVectorByAngle(vector, angle.CreateNonDirectionalAngle());
        }

        /// <summary>
        /// Creates a vector with rounding applied to each component first
        /// </summary>
        /// <param name="x">X Vector component</param>
        /// <param name="y">Y Vector component</param>
        /// <returns>Vector2 with rounded components</returns>
        public static Vector2 BuildVector2Rounded(float x, float y)
        {
            var midpointRounding = MidpointRounding.ToEven;
            return new Vector2((float)Math.Round(x, VectorOperationSignificantDigits, midpointRounding), 
                               (float)Math.Round(y, VectorOperationSignificantDigits, midpointRounding));
        }
    }
}
