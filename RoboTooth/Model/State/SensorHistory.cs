using System.Collections.Generic;
using System.Numerics;

namespace RoboTooth.Model.State
{
    /// <summary>
    /// Represents a single sensor reading
    /// </summary>
    public interface ISensorReading
    {
        /// <summary>
        /// The point in space where the measurement was made
        /// </summary>
        Vector2 MeasurementPoint { get; }

        /// <summary>
        /// The time when the measurement was made
        /// </summary>
        Duration MeasurementTime { get; }
    }

    /// <summary>
    /// Keeps track of sensor recordings taken at any given point in time/space.
    /// </summary>
    public class SensorHistory<Reading> where Reading : ISensorReading
    {
        public void HandleNewReadingAvailable(object sender, Reading newReading)
        {
            _readings.Add(newReading);
        }

        private readonly List<Reading> _readings = new List<Reading>();
    }
}
