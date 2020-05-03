using RoboTooth.Model.MessagingService.Messages.RxMessages;
using System;
using System.Numerics;

namespace RoboTooth.ViewModel.DataDisplayVM
{
    public class InternalDataDisplay : ObservableObject
    {
        private const string NOT_APPLICABLE = "N/A";

        public InternalDataDisplay()
        {
            _echoDistanceValue = NOT_APPLICABLE;
            _MagnetometerOrientationXValue = NOT_APPLICABLE;
            _MagnetometerOrientationYValue = NOT_APPLICABLE;
            _MagnetometerOrientationZValue = NOT_APPLICABLE;

            _positionX = NOT_APPLICABLE;
            _positionY = NOT_APPLICABLE;
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

        #region Echo Distance

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

        #endregion

        #region Magnetometer

        private string _MagnetometerOrientationXValue;
        public string MagnetometerOrientationXValue
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
        public string MagnetometerOrientationYValue
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
        public string MagnetometerOrientationZValue
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

        #endregion

        #region Rotary Encoders

        private uint _rotaryEncoderCounterLeft;
        public uint RotaryEncoderCounterLeft
        {
            get
            {
                return _rotaryEncoderCounterLeft;
            }
            set
            {
                _rotaryEncoderCounterLeft = value;
                NotifyPropertyChanged();
            }
        }

        private uint _rotaryEncoderCounterRight;
        public uint RotaryEncoderCounterRight
        {
            get
            {
                return _rotaryEncoderCounterRight;
            }
            set
            {
                _rotaryEncoderCounterRight = value;
                NotifyPropertyChanged();
            }
        }

        public void HandleRotaryEncodersMessage(object sender, RotaryEncodersMessage message)
        {
            //Switch to UI thread
            App.Current?.Dispatcher?.Invoke(delegate
            {
                RotaryEncoderCounterLeft = message.GetLeftWheelCounter();
                RotaryEncoderCounterRight = message.GetRightWheelCounter();
            });
        }

        #endregion
    }
}
