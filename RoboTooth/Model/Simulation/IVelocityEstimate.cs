
namespace RoboTooth.Model.Simulation
{
    /// <summary>
    /// Exposes velocity estimates.
    /// 
    /// TODO: How does this get integrated with the ISolver? Is ISolver the consumer here?
    /// </summary>
    public interface IVelocityEstimate
    {
        /// <summary>
        /// Gets the current movement velocity estimate
        /// </summary>
        /// <returns>Velocity</returns>
        float GetMovementVelocity();

        /// <summary>
        /// Gets the current rotation velocity estimate.
        /// </summary>
        /// <returns>Angular velocity</returns>
        float GetRotationVelocity();
    }
}
