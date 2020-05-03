namespace RoboTooth.Model.Control
{
    /// <summary>
    /// Interface for controlling the movement of the robot
    /// </summary>
    public interface IMovementController
    {
        /// <summary>
        /// Gets the current movement speed
        /// </summary>
        /// <returns>Percentage of the maximum speed where 1.0 is 100%
        /// </returns>
        float GetCurrentSpeedPercentage();

        /// <summary>
        /// Issues the command to move the robot forward
        /// </summary>
        /// <param name="rotationDuration">Time that the movement will take</param>
        /// <param name="speedPercentage">Percentage of the maximum movement speed,
        ///                               where 1.0 is 100%
        /// </param>
        /// <returns>ID returned by the firmware that identifies this command
        /// </returns>
        byte MoveForwardTimed(Duration movementDuration, float speedPercentage);

        /// <summary>
        /// Issues the command to rotate the robot clockwise
        /// </summary>
        /// <param name="rotationDuration">Time that the rotation will take</param>
        /// <param name="speedPercentage">Percentage of the maximum movement speed,
        ///                               where 1.0 is 100%
        /// </param>
        /// <returns>ID returned by the firmware that identifies this command
        /// </returns>
        byte TurnClockwise(Duration rotationDuration, float speedPercentage);

        /// <summary>
        /// Issues the command to rotate the robot counter clockwise
        /// </summary>
        /// <param name="rotationDuration">Time that the rotation will take</param>
        /// <param name="speedPercentage">Percentage of the maximum movement speed,
        ///                               where 1.0 is 100%
        /// </param>
        /// <returns>ID returned by the firmware that identifies this command
        /// </returns>
        byte TurnCounterClockwise(Duration rotationDuration, float speedPercentage);
    }
}
