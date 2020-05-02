using NUnit.Framework;
using RoboTooth.Model.Kinematics;
using System;

namespace RoboToothTests
{
    [TestFixture]
    public class DirectionalAngleTests
    {
        #region CreateNonDirectionalAngle

        [Test]
        public void CreateNonDirectionalAngle_RadiansClockwise()
        {
            var directionalAngle = DirectionalAngle.CreateFromRadians(Math.PI, true);

            var angle = directionalAngle.CreateNonDirectionalAngle();

            Assert.AreEqual(-Math.PI, angle.Radians);
            Assert.AreEqual(-180.0, angle.Degrees);
        }

        [Test]
        public void CreateNonDirectionalAngle_RadiansCounterClockwise()
        {
            var directionalAngle = DirectionalAngle.CreateFromRadians(Math.PI, false);

            var angle = directionalAngle.CreateNonDirectionalAngle();

            Assert.AreEqual(Math.PI, angle.Radians);
            Assert.AreEqual(180.0, angle.Degrees);
        }

        [Test]
        public void CreateNonDirectionalAngle_DegreesClockwise()
        {
            var directionalAngle = DirectionalAngle.CreateFromDegrees(180.0, true);

            var angle = directionalAngle.CreateNonDirectionalAngle();

            Assert.AreEqual(-Math.PI, angle.Radians);
            Assert.AreEqual(-180.0, angle.Degrees);
        }

        [Test]
        public void CreateNonDirectionalAngle_DegreesCounterClockwise()
        {
            var directionalAngle = DirectionalAngle.CreateFromDegrees(180.0, false);

            var angle = directionalAngle.CreateNonDirectionalAngle();

            Assert.AreEqual(Math.PI, angle.Radians);
            Assert.AreEqual(180.0, angle.Degrees);
        }

        #endregion
    }
}
