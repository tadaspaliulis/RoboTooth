using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.ViewModel.Drawing
{
    /// <summary>
    /// A readonly ViewPortSettings interface to be used
    /// by objects that should not be able to modify
    /// the settings themselves.
    /// </summary>
    public interface IViewPortSettingsReadonly
    {
        float MapScaling { get;  }
        float PanX { get; }
        float PanY { get; }
    }
}
