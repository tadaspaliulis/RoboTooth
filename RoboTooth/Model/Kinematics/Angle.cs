using System;

namespace RoboTooth.Model.Kinematics
{
    /// <summary>
    /// Represents an angle.
    /// Abstracts away degrees and radiands.
    /// </summary>
    public class Angle
    {
        protected Angle()
        {

        }

        protected double? _radians;
        protected double? _degrees;

        public double Radians
        {
            get
            {
                if (_radians != null)
                    return _radians.Value;

                //Check for null value?
                return ConvertDegreesToRadians(_degrees.Value);
            }
        }

        public double Degrees
        {
            get
            {
                if (_degrees != null)
                    return _degrees.Value;

                //Check for null value?
                return ConvertRadiandsToDegrees(_radians.Value);
            }
        }

        private static double ConvertRadiandsToDegrees(double radians)
        {
            return (radians / Math.PI) * 180.0;
        }

        private static double ConvertDegreesToRadians(double degrees)
        {
            return (degrees / 180.0) * Math.PI;
        }

        #region Factories

        public static Angle CreateFromRadians(double radians)
        {
            return new Angle
            {
                _radians = radians
            };
        }

        public static Angle CreateFromDegrees(double degrees)
        {
            return new Angle
            {
                _degrees = degrees
            };
        }

        #endregion
    }

    /// <summary>
    /// An angle with directional information (clockwise or counter-clockwise).
    /// </summary>
    public class DirectionalAngle : Angle
    {
        public bool IsClockwise { get; set; }

        /// <summary>
        /// Creates a non-directional angle from the directional one.
        /// The direction is converted a positive angle for counter-clockwise and 
        /// a negative one for clockwise angles
        /// </summary>
        /// <returns>A non-directional angle</returns>
        public Angle CreateNonDirectionalAngle()
        {
            if (IsClockwise)
            {
                if (_degrees.HasValue)
                {
                    return CreateFromDegrees(-_degrees.Value);
                }
                else
                {
                    return CreateFromRadians(-_radians.Value);
                }
            }
            else
            {
                if (_degrees.HasValue)
                {
                    return CreateFromDegrees(_degrees.Value);
                }
                else
                {
                    return CreateFromRadians(_radians.Value);
                }
            }
        }

        #region Factories

        public static DirectionalAngle CreateFromRadians(double radians, bool clockwise)
        {
            return new DirectionalAngle
            {
                _radians = radians,
                IsClockwise = clockwise
            };
        }

        public static DirectionalAngle CreateFromDegrees(double degrees, bool clockwise)
        {
            return new DirectionalAngle
            {
                _degrees = degrees,
                IsClockwise = clockwise
            };
        }

        #endregion
    }
}
