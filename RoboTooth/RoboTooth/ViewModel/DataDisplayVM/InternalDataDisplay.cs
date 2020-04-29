using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using RoboTooth.Model.MessagingService.Messages.RxMessages;
namespace RoboTooth.ViewModel.DataDisplayVM
{
    public class InternalDataDisplay : ObservableObject
    {
        public InternalDataDisplay()
        {
            _echoDistanceValue = "N/A";
            _MagnetometerOrientationXValue = "N/A";
            _MagnetometerOrientationYValue = "N/A";
            _MagnetometerOrientationZValue = "N/A";

            _targetPositionX = 0;
            _targetPositionY = 0;
        }

        #region Position

        private string _positionX;
        public string PositionX
        {
            get { return _positionX; }
            set
            {
                _positionX = value;
                NotifyPropertyChanged();
            }
        }

        private string _positionY;
        public string PositionY
        {
            get { return _positionY; }
            set
            {
                _positionY = value;
                NotifyPropertyChanged();
            }
        }

        private float _targetPositionX;
        public float TargetPositionX
        {
            get { return _targetPositionX; }
            set
            {
                _targetPositionX = value;
                NotifyPropertyChanged();
            }
        }

        private float _targetPositionY;
        public float TargetPositionY
        {
            get { return _targetPositionY; }
            set
            {
                _targetPositionY = value;
                NotifyPropertyChanged();
            }
        }

        public void HandlePositionUpdated(Vector2 position)
        {
            //Switch to UI thread
            App.Current?.Dispatcher?.Invoke(delegate
            {
                PositionX = position.X.ToString();
                PositionY = position.Y.ToString();
            });
        }

        #endregion

        private string _echoDistanceValue;
        public String EchoDistanceValue
        {
            get
            {
                return _echoDistanceValue;
            }
            set
            {
                _echoDistanceValue = value;
                NotifyPropertyChanged();
            }
        }

        public void HandleEchoDistanceMessage(object sender, EchoDistanceMessage message)
        {
            //Switch to UI thread
            App.Current?.Dispatcher?.Invoke(delegate
            {
                EchoDistanceValue = message.GetDistance().ToString();
            });
        }

        private string _MagnetometerOrientationXValue;
        public String MagnetometerOrientationXValue
        {
            get
            {
                return _MagnetometerOrientationXValue;
            }
            set
            {
                _MagnetometerOrientationXValue = value;
                NotifyPropertyChanged();
            }
        }

        private string _MagnetometerOrientationYValue;
        public String MagnetometerOrientationYValue
        {
            get
            {
                return _MagnetometerOrientationYValue;
            }
            set
            {
                _MagnetometerOrientationYValue = value;
                NotifyPropertyChanged();
            }
        }

        private string _MagnetometerOrientationZValue;
        public String MagnetometerOrientationZValue
        {
            get
            {
                return _MagnetometerOrientationZValue;
            }
            set
            {
                _MagnetometerOrientationZValue = value;
                NotifyPropertyChanged();
            }
        }

        public void HandleMagnetometerOrientationMessage(object sender, MagnetometerOrientationMessage message)
        {
            //Switch to UI thread
            App.Current?.Dispatcher?.Invoke(delegate
            {
                MagnetometerOrientationXValue = message.GetX().ToString();
                MagnetometerOrientationYValue = message.GetY().ToString();
                MagnetometerOrientationZValue = message.GetZ().ToString();
            });
        }
    }
}
