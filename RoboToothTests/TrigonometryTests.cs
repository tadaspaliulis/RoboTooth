using Extensions;
using NUnit.Framework;
using RoboTooth.Model.Kinematics;
using System.Numerics;

namespace RoboToothTests
{
    /// <summary>
    /// Unit tests for the Trigonometry helper functions.
    /// </summary>
    [TestFixture]
    public class TrigonometryTests
    {
        #region CalculateAngle tests

        [Test]
        public void CalculateAngle_RightAngle()
        {
            var v1 = new Vector2(0.0f, 1.0f);
            var v2 = new Vector2(1.0f, 0.0f);

            var angle = Trigonometry.CalculateAngle(v1, v2);

            Assert.AreEqual(90.0, angle.Degrees, double.Epsilon);
        }

        [Test]
        public void CalculateAngle_SameDirection()
        {
            var v1 = new Vector2(0.0f, 1.0f);
            var v2 = new Vector2(0.0f, 1.0f);

            var angle = Trigonometry.CalculateAngle(v1, v2);

            Assert.AreEqual(0.0, angle.Degrees, double.Epsilon);
        }

        [Test]
        public void CalculateAngle_OppositeDirection()
        {
            var v1 = new Vector2(0.0f, 1.0f);
            var v2 = new Vector2(0.0f, -1.0f);

            var angle = Trigonometry.CalculateAngle(v1, v2);

            Assert.AreEqual(180.0, angle.Degrees, double.Epsilon);
        }

        #endregion

        #region CalculateDirectionalAngle

        [Test]
        public void CalculateDirectionalAngle_RightAngleClockwise()
        {
            var v1 = new Vector2(0.0f, 1.0f);
            var v2 = new Vector2(1.0f, 0.0f);

            var angle = Trigonometry.CalculateDirectionalAngle(v1, v2);

            Assert.AreEqual(90.0, angle.Degrees, double.Epsilon);
            Assert.IsTrue(angle.IsClockwise);
        }

        [Test]
        public void CalculateDirectionalAngle_RightAngleAntiClockwise()
        {
            var v1 = new Vector2(1.0f, 0.0f);
            var v2 = new Vector2(0.0f, 1.0f);

            var angle = Trigonometry.CalculateDirectionalAngle(v1, v2);

            Assert.AreEqual(90.0, angle.Degrees, double.Epsilon);
            Assert.IsFalse(angle.IsClockwise);
        }

        [Test]
        public void CalculateDirectionalAngle_OppositeDirectionClockwise()
        {
            var v1 = new Vector2(0.0f, -1.0f);
            var v2 = new Vector2(0.0f, 1.0f);

            var angle = Trigonometry.CalculateDirectionalAngle(v1, v2);

            Assert.AreEqual(180.0, angle.Degrees, double.Epsilon);
            Assert.IsFalse(angle.IsClockwise);
        }

        [Test]
        public void CalculateDirectionalAngle_45DegreeDiagonalClockwise()
        {
            var v1 = Vector2.Normalize(new Vector2(1.0f, 1.0f));
            var v2 = new Vector2(1.0f, 0.0f);

            var angle = Trigonometry.CalculateDirectionalAngle(v1, v2);

            Assert.AreEqual(45.0, angle.Degrees, 1e-5);
            Assert.IsTrue(angle.IsClockwise);
        }

        [Test]
        public void CalculateDirectionalAngle_135DegreeDiagonalCounterClockwise()
        {
            var v1 = Vector2.Normalize(new Vector2(1.0f, 1.0f));
            var v2 = new Vector2(-1.0f, 0.0f);

            var angle = Trigonometry.CalculateDirectionalAngle(v1, v2);

            Assert.AreEqual(135.0, angle.Degrees, 1e-5);
            Assert.IsFalse(angle.IsClockwise);
        }

        [Test]
        public void CalculateDirectionalAngle_135DegreeDiagonalClockwise()
        {
            var v1 = new Vector2(1.0f, 0.0f);
            var v2 = Vector2.Normalize(new Vector2(-1.0f, -1.0f));

            var angle = Trigonometry.CalculateDirectionalAngle(v1, v2);

            Assert.AreEqual(135.0, angle.Degrees, 1e-5);
            Assert.IsTrue(angle.IsClockwise);
        }

        /// <summary>
        /// TODO: need better name here
        /// </summary>
        [Test]
        public void CalculateDirectionalAngle_135DegreeDiagonalClockwise_2()
        {
            var v1 = Vector2.Normalize(new Vector2(-1.0f, -1.0f));
            var v2 = new Vector2(1.0f, 0.0f);

            var angle = Trigonometry.CalculateDirectionalAngle(v1, v2);

            Assert.AreEqual(135.0, angle.Degrees, 1e-5);
            Assert.IsFalse(angle.IsClockwise);
        }

        #endregion

        #region RotateVectorByAngle

        [Test]
        public void RotateVectorByAngle_RightAngleCounterClockwise()
        {
            var v1 = new Vector2(0.0f, 1.0f);
            var rotatedVector = Trigonometry.RotateVectorByAngle(v1, Angle.CreateFromDegrees(90));

            Assert.IsTrue(rotatedVector.EqualsWithinDelta(new Vector2(-1.0f, 0.0f), float.Epsilon));
        }

        [Test]
        public void RotateVectorByAngle_RightAngleClockwise()
        {
            var v1 = new Vector2(0.0f, 1.0f);
            var rotatedVector = Trigonometry.RotateVectorByAngle(v1, Angle.CreateFromDegrees(-90));

            Assert.IsTrue(rotatedVector.EqualsWithinDelta(new Vector2(1.0f, 0.0f), float.Epsilon));
        }

        [Test]
        public void RotateVectorByAngle_HalfCircleClockwise()
        {
            var v1 = new Vector2(0.0f, 1.0f);
            var rotatedVector = Trigonometry.RotateVectorByAngle(v1, Angle.CreateFromDegrees(180));

            Assert.IsTrue(rotatedVector.EqualsWithinDelta(new Vector2(0.0f, -1.0f), float.Epsilon));
        }

        [Test]
        public void RotateVectorByAngle_HalfCircleCounterClockwise()
        {
            var v1 = new Vector2(0.0f, 1.0f);
            var rotatedVector = Trigonometry.RotateVectorByAngle(v1, Angle.CreateFromDegrees(-180));

            Assert.IsTrue(rotatedVector.EqualsWithinDelta(new Vector2(0.0f, -1.0f), float.Epsilon));
        }

        [Test]
        public void RotateVectorByAngle_DiagonalRightAngleClockwise()
        {
            var v1 = new Vector2(1.0f, 1.0f);
            var rotatedVector = Trigonometry.RotateVectorByAngle(v1, Angle.CreateFromDegrees(-90));

            Assert.IsTrue(rotatedVector.EqualsWithinDelta(new Vector2(1.0f, -1.0f), float.Epsilon));
        }

        [Test]
        public void RotateVectorByAngle_DiagonalRightAngleCounterClockwise()
        {
            var v1 = new Vector2(1.0f, 1.0f);
            var rotatedVector = Trigonometry.RotateVectorByAngle(v1, Angle.CreateFromDegrees(90));

            Assert.IsTrue(rotatedVector.EqualsWithinDelta(new Vector2(-1.0f, 1.0f), float.Epsilon));
        }

        #endregion
    }
}
