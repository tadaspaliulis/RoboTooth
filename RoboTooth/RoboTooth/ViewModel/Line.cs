using RoboTooth.ViewModel.Drawing;
using System.ComponentModel;
using System.Numerics;

namespace RoboTooth.ViewModel
{
    /// <summary>
    /// View Model of a line
    /// </summary>
    public class Line : DrawableObservable
    {
        public Line() { }

        public Line(Vector2 StartPoint, Vector2 EndPoint)
        {
            _startPointX = StartPoint.X;
            _startPointY = StartPoint.Y;

            _endPointX = EndPoint.X;
            _endPointY = EndPoint.Y;
        }

        #region Observable Properties
        private float _startPointX;
        public float StartPointX
        {
            get
            {
                return _startPointX * CurrentViewPortSettings.MapScaling
                    + CurrentViewPortSettings.PanX;
            }
            set
            {

                _startPointX = value;
                NotifyPropertyChanged();
            }
        }

        private float _startPointY;
        public float StartPointY
        {
            get
            {
                return _startPointY * CurrentViewPortSettings.MapScaling
                    + CurrentViewPortSettings.PanY;
            }
            set
            {
                _startPointY = value;
                NotifyPropertyChanged();
            }
        }

        private float _endPointX;
        public float EndPointX
        {
            get
            {
                return _endPointX * CurrentViewPortSettings.MapScaling
                    + CurrentViewPortSettings.PanX;
            }
            set
            {
                _endPointX = value;
                NotifyPropertyChanged();
            }
        }

        private float _endPointY;
        public float EndPointY
        {
            get
            {
                return _endPointY * CurrentViewPortSettings.MapScaling
                    + CurrentViewPortSettings.PanY;
            }
            set
            {
                _endPointY = value;
                NotifyPropertyChanged();
            }
        }

        public float _opacity = 1.0f;
        public float Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public override void HandleViewPortChange(object sender, PropertyChangedEventArgs e)
        {
            // Force the line to be redrawn
            NotifyPropertyChanged(nameof(StartPointX));
            NotifyPropertyChanged(nameof(StartPointY));
            NotifyPropertyChanged(nameof(EndPointX));
            NotifyPropertyChanged(nameof(EndPointY));
        }
    }
}
