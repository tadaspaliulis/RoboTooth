﻿using RoboTooth.Model.Control.Filters;
using RoboTooth.Model.Control.Sensors;
using RoboTooth.Model.Kinematics;
using RoboTooth.Model.MessagingService;
using RoboTooth.Model.MessagingService.Messages.RxMessages;
using RoboTooth.Model.MessagingService.Messages.TxMessages;
using RoboTooth.Model.Simulation;
using RoboTooth.Model.State;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace RoboTooth.Model.Control
{

    public class MotorCalibration
    {
        public MotorCalibration(float MaxMovementVelocity, float MaxRotationVelocityDegrees)
        {
            this.MaxMovementVelocity = MaxMovementVelocity;
            this.MaxRotationVelocityDegrees = MaxRotationVelocityDegrees;
        }

        public readonly float MaxMovementVelocity;
        public readonly float MaxRotationVelocityDegrees;
    }

    /// <summary>
    /// Object responsible for controlling the robot and dealing with the data retrieved from it
    /// </summary>
    public class RoboController
    {
        public RoboController(MessagingService.MessagingService messagingService, MessageSorter messageSorter, MotionHistory motionHistory)
        {
            _messagingService = messagingService;

            //Subscribe to various message events from the message sorter.
            _messageSorter = messageSorter;

            _messageSorter.MagnetometerOrientationMessages.MessageReceived += handleMagnetometerOrientationMessage;
            _messageSorter.DebugStringMessages.MessageReceived += handleDebugStringMessage;

            _motorsSimulator = new MotorSimulator();
            _motorsController = new MotorsController(_messagingService, _motorsSimulator);
            _messageSorter.ActionCompletedMessages.MessageReceived += _motorsController.HandleActionCompletedMessage;

            var calibration = new MotorCalibration(MaxMovementVelocity: 10,
                                                   MaxRotationVelocityDegrees: 280);

            _motionHistory = motionHistory;
            _solver = new SolverNaive(calibration.MaxMovementVelocity, calibration.MaxRotationVelocityDegrees);

            _kinematicsModel = new KinematicsModel(_motorsSimulator, _solver);
            _locomotionPlanner = new LocomotionPlanner(_solver, _kinematicsModel, _motorsSimulator, _motorsController, 5.0f, Angle.CreateFromDegrees(5.0));
            _navigationPlanner = new NavigationPlanner(_kinematicsModel, _locomotionPlanner, _motionHistory);

            //Subscribe motion history to kinematics events.
            //TODO: Unify event handler names. Currently it's a mix of On... and Handle.. styles.
            _kinematicsModel.CurrentOrientationUpdated += motionHistory.OnOrientationChangedEvent;
            _kinematicsModel.CurrentPositionUpdated += motionHistory.OnPositionChangedEvent;

            // Set up the sensors
            _echoDistanceSensor = new EchoDistanceSensor(_kinematicsModel, new RunningAverageFilter(3), Vector2.Zero);
            _messageSorter.EchoDistanceMessages.MessageReceived += _echoDistanceSensor.HandleRawSensorDataReceived;

            _magnetometer = new Magnetometer();
            _messageSorter.MagnetometerOrientationMessages.MessageReceived += _magnetometer.HandleRawSensorDataReceived;

            StartExploration();
        }

        public void Test()
        {
            _navigationPlanner.Test();
        }

        public void MoveToLocation(float targetX, float targetY)
        {
            //TODO: WARNING. This currently has no syncronisation with other movement stuff.
            _navigationPlanner.MoveToPosition(new Vector2(targetX, targetY), 1.0f);
        }

        #region Robot Message handlers

        private void handleMagnetometerOrientationMessage(object sender, MagnetometerOrientationMessage message)
        {
            var val = Math.Atan2(message.GetY() - 1800, message.GetZ() + 600);
            //if (val < 0)
            //    val = -val;

            val = val * 180 / Math.PI;
            //System.Diagnostics.Debug.WriteLine(val);
        }

        private void handleDebugStringMessage(object sender, DebugStringMessage message)
        {
            System.Diagnostics.Debug.WriteLine(message.GetDebugString());
        }

        #endregion

        #region Robot Actions

        private void performRobotMovementAction(TimedMoveMessage action)
        {
            //_motorActionQueue.AddActionToQueue(action);
            //_messagingService.SendMessage(action);
        }

        public void TurnLeftIndefinite()
        {
            performRobotMovementAction(new IndefiniteMoveMessage(MoveDirection.ETurnLeft, 255));
        }

        public void TurnRightIndefinite()
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnRight, 255, 5000));
        }

        public void MoveForwardIndefinite()
        {
            performRobotMovementAction(new IndefiniteMoveMessage(MoveDirection.EForward, 255));
            _navigationPlanner.Test();
        }

        public void MoveBackwardsIndefinite()
        {
            performRobotMovementAction(new IndefiniteMoveMessage(MoveDirection.EBackwards, 255));
        }

        public void StopMovement()
        {
            performRobotMovementAction(new IndefiniteMoveMessage(MoveDirection.EStop, 0));
        }

        public void TurnClockwise(short degrees)
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnRight, 255, getRotationTime(degrees)));
        }

        public void TurnCounterClockwise(short degrees)
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnLeft, 255, getRotationTime(degrees)));
        }

        public void MoveForwardTimed(ushort durationMicroS)
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnLeft, 255, durationMicroS));
        }

        public void MoveBackwardsTimed(ushort durationMicroS)
        {
            performRobotMovementAction(new TimedMoveMessage(MoveDirection.ETurnLeft, 255, durationMicroS));
        }

        #endregion

        private bool _isExplorationRunning = false;

        public void StartExploration()
        {
            //Make sure the simulation is not already running.
            if (true == _isExplorationRunning)
                throw new InvalidOperationException("Exploration is already running.");

            _isExplorationRunning = true;

            Task simulation = Task.Factory.StartNew(RunSimulationLoop);

            //_kinematicsModel.Simulate
        }

        public IPositionState GetPositionState()
        {
            return _kinematicsModel;
        }

        //private CancellationTokenSource _cancelationTokenSource;

        private void RunSimulationLoop(/*CancellationToken token*/)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true)
            {
                //if (token.IsCancellationRequested)
                //   break;

                //Don't want to call the simulation too frequently,
                //because it might blow up due to floating point inaccuracy accumulation
                //and hog the processing time.
                var deltaTime = Duration.CreateFromMiliSeconds(stopWatch.ElapsedMilliseconds);
                if (deltaTime.Miliseconds < 5)
                {
                    Thread.Sleep(1);
                    continue;
                }

                stopWatch.Restart();
                //Console.WriteLine($"Calling simulate with dT={deltaTime.Seconds} seconds");
                _motorsSimulator.Simulate(deltaTime);
                _kinematicsModel.Simulate(deltaTime);
            }
        }

        /// <summary>
        /// Calculate how much time robot would need to rotate by specified amount
        /// </summary>
        /// <param name="degrees">An angle for turning</param>
        /// <returns>Number of microseconds needed to turn by specified angle</returns>
        private ushort getRotationTime(short degrees)
        {
            return (ushort)(((float)degrees / 360) * _timeToDo360MicroSeconds);
        }

        #region Private variables

        const float _timeToDo360MicroSeconds = 3600; //Todo, need to figure out what this value actually is

        private readonly MessagingService.MessagingService _messagingService;
        private readonly MessageSorter _messageSorter;

        private readonly MotorSimulator _motorsSimulator;
        private readonly MotorsController _motorsController;

        private readonly MotionHistory _motionHistory;

        private readonly ISolver _solver;

        private readonly KinematicsModel _kinematicsModel;

        private readonly LocomotionPlanner _locomotionPlanner;

        private readonly NavigationPlanner _navigationPlanner;

        private readonly EchoDistanceSensor _echoDistanceSensor;
        private readonly Magnetometer _magnetometer;
        #endregion
    }
}
