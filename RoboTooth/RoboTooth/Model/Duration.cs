using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model
{
    /// <summary>
    /// Abstracts away time spans and provides abstractions
    /// for dealing with various units.
    /// </summary>
    public class Duration
    {
        protected Duration() { }

        public long Miliseconds
        {
            get
            {
                if (_miliseconds.HasValue)
                    return _miliseconds.Value;

                return (long)(_seconds.Value * 1000);
            }
        }

        public float Seconds
        {
            get
            {
                if (_seconds.HasValue)
                    return _seconds.Value;

                return (float)_miliseconds.Value / 1000;
            }
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

        private float? _seconds;

        private long? _miliseconds;
    }
}
