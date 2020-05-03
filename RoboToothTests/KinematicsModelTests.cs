using Moq;
using NUnit.Framework;
using RoboTooth.Model;
using RoboTooth.Model.Control;
using RoboTooth.Model.Kinematics;
using System.Numerics;

namespace RoboToothTests
{
    [TestFixture]
    public class KinematicsModelTests
    {
        [Test]
        public void Simulate_MotorsNotActive()
        {
            var solverMock = Mock.Of<ISolver>();
            var motorStateMock = Mock.Of<IMotorState>(p => p.GetCurrentSpeedPercentage() == 0.0);

            var model = new KinematicsModel(motorStateMock, solverMock);

            model.Simulate(Duration.CreateFromSeconds(1.0f));

            Assert.AreEqual(model.GetCurrentPosition(), Vector2.Zero);
            Assert.AreEqual(model.GetCurrentOrientation(), Vector2.UnitY);
        }
    }
}
