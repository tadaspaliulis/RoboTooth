using NUnit.Framework;
using System;
using RoboTooth.Model.Kinematics;

namespace RoboToothTests
{
    [TestFixture]
    public class AngleTests
    {
        #region CreateFromRadians

        [Test]
        public void CreateFromRadiansZero()
        {
            var angle = Angle.CreateFromRadians(0.0);
            Assert.AreEqual(0.0, angle.Radians);
            Assert.AreEqual(0.0, angle.Degrees, float.Epsilon);
        }

        [Test]
        public void CreateFromRadiansPI()
        {
            var angle = Angle.CreateFromRadians(Math.PI);
            Assert.AreEqual(Math.PI, angle.Radians);
            Assert.AreEqual(180.0, angle.Degrees, float.Epsilon);
        }

        [Test]
        public void CreateFromRadiansHalfPI()
        {
            var angle = Angle.CreateFromRadians(Math.PI / 2);
            Assert.AreEqual(Math.PI / 2, angle.Radians);
            Assert.AreEqual(90.0, angle.Degrees, float.Epsilon);
        }

        [Test]
        public void CreateFromRadiansHalf2PI()
        {
            var angle = Angle.CreateFromRadians(Math.PI * 2);
            Assert.AreEqual(Math.PI * 2, angle.Radians);
            Assert.AreEqual(360.0, angle.Degrees, float.Epsilon);
        }

        #endregion

        #region CreateFromDegrees
    
        [Test]
        public void CreateFromDegreesZero()
        {
            var angle = Angle.CreateFromDegrees(0.0);
            Assert.AreEqual(0.0, angle.Degrees);
            Assert.AreEqual(0.0, angle.Radians);
        }

        [Test]
        public void CreateFromDegreesRightAngle()
        {
            var angle = Angle.CreateFromDegrees(90.0);
            Assert.AreEqual(90.0, angle.Degrees);
            Assert.AreEqual(Math.PI / 2, angle.Radians, double.Epsilon);
        }

        [Test]
        public void CreateFromDegreesHalfCircle()
        {
            var angle = Angle.CreateFromDegrees(180.0);
            Assert.AreEqual(180.0, angle.Degrees);
            Assert.AreEqual(Math.PI, angle.Radians, double.Epsilon);
        }

        [Test]
        public void CreateFromDegreesCircle()
        {
            var angle = Angle.CreateFromDegrees(360.0);
            Assert.AreEqual(360.0, angle.Degrees);
            Assert.AreEqual(Math.PI * 2, angle.Radians, double.Epsilon);
        }

        #endregion
    }
}
