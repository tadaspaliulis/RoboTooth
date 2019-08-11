using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.ViewModel.Drawing
{
    /// <summary>
    /// TODO
    /// </summary>
    public class ViewPortSettings : ObservableObject, IViewPortSettingsReadonly
    {
        private float _mapScaling = 1.0f;
        public float MapScaling
        {
            get { return _mapScaling; }
            set
            {
                _mapScaling = value;
                NotifyPropertyChanged();
            }
        }

        private float _panX = 150.0f;

        public float PanX
        {
            get { return _panX; }
            set
            {
                _panX = value;
                NotifyPropertyChanged();
            }
        }

        private float _panY = 150.0f;

        public float PanY
        {
            get { return _panY; }
            set
            {
                _panY = value;
                NotifyPropertyChanged();
            }
        }
    }
}
