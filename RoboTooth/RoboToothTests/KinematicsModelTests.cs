using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using RoboTooth.Model.Kinematics;
using RoboTooth.Model.Control;
using System.Numerics;
using RoboTooth.Model;

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
