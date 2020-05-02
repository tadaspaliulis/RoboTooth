using System.Numerics;

namespace RoboTooth.Model.Kinematics
{
    /// <summary>
    /// TODO: Needs to be cleaned up. What is the purpose of this interface?
    /// Perhaps solver is not a great name choice?
    /// </summary>
    public interface ISolver
    {
        float GetCurrentMovementSpeed(float speedPercentage);
        float GetCurrentRotationSpeed(float speedPercentage);

        Duration CalculateMovementDurationForDeltaDistance(Vector2 deltaDistance, float motorSpeedPercentage);
        Vector2 CalculateMovementVector(Vector2 orientation, float motorSpeedPercent, Duration duration);
        Duration CalculateRotationDurationForNewOrientation(Vector2 initialOrientation, 
                                                            Vector2 newOrientation, 
                                                            float motoSpeedPercentage, 
                                                            out bool rotateClockwise);

        DirectionalAngle CalculateDeltaRotation(float speedPercentage, Duration deltaTime, bool clockwise);
    }
}