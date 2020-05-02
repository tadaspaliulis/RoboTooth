using RoboTooth.Model.Kinematics;
using RoboTooth.Model.MessagingService.Messages.RxMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


            var heading =  Math.Atan2((double)((message.GetZ() - _offsetZ) * zScaling),
                                        (double)((message.GetY() - _offsetY) * yScaling));
                    
            //So is this relative to start position?
            var angularHeading = Angle.CreateFromRadians(heading);
            Console.WriteLine($"Y={message.GetY()} Z={message.GetZ()} Heading: {angularHeading.Degrees + 180}");
        }

        private float yScaling = 0.0031446540880503146f;
        private float zScaling = 0.0030303030303030303f;

        private short _offsetY = 1886;
        private short _offsetZ = -320;
    }
}
