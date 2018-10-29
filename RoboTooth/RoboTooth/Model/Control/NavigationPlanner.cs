using RoboTooth.Model.Kinematics;
using RoboTooth.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.Control
{
    /// <summary>
    /// 
    /// </summary>
    public class NavigationPlanner
    {
        public NavigationPlanner(KinematicsModel kinematicsModel, MotorsController motorsController, MotionHistory motionHistory)
        {
            _kinematicsModel = kinematicsModel;
            _motorsController = motorsController;
            _motionHistory = motionHistory;
        }

        public void MoveToPosition(Vector2 newPosition)
        {

        }

        private KinematicsModel _kinematicsModel;
        private MotorsController _motorsController;
        private MotionHistory _motionHistory;
    }
}
