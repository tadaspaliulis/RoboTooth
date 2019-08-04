using System;
using System.Collections.Generic;

namespace RoboTooth.Model.Control.Filters
{
    /// <summary>
    /// TODO
    /// </summary>
    class RunningAverageFilter : IFilter<float>
    {
        public RunningAverageFilter(int valuesRequiredForAverage)
        {
            _valuesRequiredForAverage = valuesRequiredForAverage;
        }

        public void HandleNewDataReceived(float newData)
        {
            _currentValues.Add(newData);
            if( _currentValues.Count >= _valuesRequiredForAverage )
            {
                AveragedDataAvailable(this, CalculateAverage());
                Reset();
            }
        }

        public void Reset()
        {
            _currentValues.Clear();
        }

        public event EventHandler<float> AveragedDataAvailable;

        #region Private methods

        private float CalculateAverage()
        {
            float sum = 0.0f;
            _currentValues.ForEach((a) => sum += a);

            return sum / _currentValues.Count;
        }

        private 

        #endregion

        #region Private variables

        List<float> _currentValues = new List<float>();

        public readonly int _valuesRequiredForAverage;

        #endregion
    }
}
