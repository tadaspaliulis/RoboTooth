using System;

namespace RoboTooth.Model
{
    /// <summary>
    /// Abstracts away time spans and provides abstractions
    /// for dealing with various units.
    /// </summary>
    public class Duration
    {
        protected Duration() { }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">Duration that will get copied</param>
        public Duration(Duration other)
        {
            _seconds = other._seconds;
            _miliseconds = other._miliseconds;
        }

        public long Miliseconds
        {
            get
            {
                if (_miliseconds.HasValue)
                    return _miliseconds.Value;

                return SecondsToMiliseconds(_seconds.Value);
            }
        }

        public float Seconds
        {
            get
            {
                if (_seconds.HasValue)
                    return _seconds.Value;

                return MilisecondsToSeconds(_miliseconds.Value);
            }
        }

        /// <summary>
        /// Creates a new object that is the result of the substraction.
        /// </summary>
        /// <param name="other">Duration that will be substracted.</param>
        /// <returns>Result of the substraction</returns>
        public Duration Substract(Duration other)
        {
            if (other == null)
                throw new ArgumentNullException("other", "Cannot substract null from a duration.");

            if (_seconds.HasValue)
            {
                _seconds -= other.Seconds;
            }
            else if (_miliseconds.HasValue)
            {
                return CreateFromMiliSeconds(_miliseconds.Value - other.Miliseconds);
            }

            throw new InvalidOperationException("Attempting to substract from a duration which " +
                                                " does not have any values defined");
        }

        #region Factories

        public static Duration CreateFromSeconds(float seconds)
        {
            return new Duration
            {
                _seconds = seconds
            };
        }

        public static Duration CreateFromMiliSeconds(long miliseconds)
        {
            return new Duration
            {
                _miliseconds = miliseconds
            };
        }

        #endregion

        #region Converters

        private const long MilisecondsInSecond = 1000;

        private static long SecondsToMiliseconds(float seconds)
        {
            return (long)(seconds * MilisecondsInSecond);
        }

        private static float MilisecondsToSeconds(long miliseconds)
        {
            return (float)miliseconds / MilisecondsInSecond;
        }

        #endregion

        private float? _seconds;

        private long? _miliseconds;
    }
}
