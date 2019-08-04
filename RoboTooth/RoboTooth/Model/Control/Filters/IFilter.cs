using System;

namespace RoboTooth.Model.Control.Filters
{
    /// <summary>
    /// Interface for sensor input filtering 
    /// </summary>
    public interface IFilter<T>
    {
        /// <summary>
        /// TODOs
        /// </summary>
        /// <param name="newData"></param>
        void HandleNewDataReceived(T newData);

        /// <summary>
        /// TODO
        /// </summary>
        void Reset();

        /// <summary>
        /// TODO
        /// </summary>
        event EventHandler<T> AveragedDataAvailable;
    }
}
