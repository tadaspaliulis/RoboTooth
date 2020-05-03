using RoboTooth.Model.Kinematics;
using RoboTooth.Model.MessagingService.Messages.RxMessages;
using System;

namespace RoboTooth.Model.Control.Sensors
{
    public class MagnetometerMeasurement
    {

    }

    public class Magnetometer
    {
        public void HandleRawSensorDataReceived(object sender, MagnetometerOrientationMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));


            var heading = Math.Atan2((double)((message.GetZ() - _offsetZ) * zScaling),
                                        (double)((message.GetY() - _offsetY) * yScaling));

            //So is this relative to start position?
            var angularHeading = Angle.CreateFromRadians(heading);
            Console.WriteLine($"Y={message.GetY()} Z={message.GetZ()} Heading: {angularHeading.Degrees + 180}");
        }

        private readonly float yScaling = 0.0031446540880503146f;
        private readonly float zScaling = 0.0030303030303030303f;

        private readonly short _offsetY = 1886;
        private readonly short _offsetZ = -320;
    }
}
