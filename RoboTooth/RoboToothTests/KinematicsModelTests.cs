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

        /// <summary>
        /// TODO: This is too integration like and not unit test like.
        /// </summary>
        [Test]
        public void Simulate_MotorsRotateClockwise()
        {
            //Not mocking this for, but perhaps should?
            var solver = new SolverNaive(1.0f, 1.0f);
            var motorStateMock = Mock.Of<IMotorState>(p => p.GetCurrentSpeedPercentage() == 1.0 && 
                                                           p.GetCurrentDirection() == MotorState.RotateClockwise);

            var model = new KinematicsModel(motorStateMock, solver);

            var totalDuration = solver.CalculateRotationDurationForNewOrientation(Vector2.UnitY, -Vector2.UnitY, 1.0f, out bool clockwise);
            var deltaTime = Duration.CreateFromSeconds(totalDuration.Seconds / 1000);

            for(int i= 0; i < 1000; ++i)
            {
                model.Simulate(deltaTime);
            }

            Assert.AreEqual(Vector2.Zero, model.GetCurrentPosition());
            Assert.AreEqual(Vector2.UnitY, model.GetCurrentOrientation());
        }

    }
}
