using System;
using System.Numerics;

namespace RoboTooth.Model.Kinematics
{
    /// <summary>
    /// Interface for retrieving position and orientation information
    /// on the robot.
    /// </summary>
    public interface IPositionState
    {
        Vector2 GetCurrentPosition();
        Vector2 GetCurrentOrientation();

        event Action<Vector2> CurrentPositionUpdated;
        event Action<Vector2> CurrentOrientationUpdated;
    }
}
