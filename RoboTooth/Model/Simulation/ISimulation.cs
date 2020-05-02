using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.Model.Simulation
{
    public interface ISimulation
    {
        void Simulate(Duration deltaTime);
    }
}
