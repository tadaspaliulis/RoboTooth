using RoboTooth.ViewModel.Commands;
using RoboTooth.ViewModel.Commands.CanExecuteEvalTriggers;
using System;

namespace RoboTooth.ViewModel.Drawing
{
    /// <summary>
    /// View Model of the viewport scaling and panning settings
    /// </summary>
    public class ViewPortSettings : ObservableObject, IViewPortSettingsReadonly
    {
        public ViewPortSettings()
        {
            var zoomInButtonCommand = new Command((a) => IsZoomInEnabled(), (a) => ZoomIn());
            zoomInButtonCommand.AddCanExecuteChangedTrigger(new PropertyChangedCanExecuteTrigger(nameof(MapScaling), this));
            _zoomInButton = new ObservableButton(zoomInButtonCommand, null);

            var zoomOutButtonCommand = new Command((a) => IsZoomOutEnabled(), (a) => ZoomOut());
            zoomOutButtonCommand.AddCanExecuteChangedTrigger(new PropertyChangedCanExecuteTrigger(nameof(MapScaling), this));
            _zoomOutButton = new ObservableButton(zoomOutButtonCommand, null);

            var resetButtonCommand = new Command((a) => IsZoomResetEnabled(), (a) => ResetZoom());
            resetButtonCommand.AddCanExecuteChangedTrigger(new PropertyChangedCanExecuteTrigger(nameof(MapScaling), this));
            _zoomResetButton = new ObservableButton(resetButtonCommand, null);
        }

        #region Observable Properties
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

        private ObservableButton _zoomInButton;
        public ObservableButton ZoomInButton
        {
            get
            {
                return _zoomInButton;
            }
            set
            {
                _zoomInButton = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableButton _zoomOutButton;
        public ObservableButton ZoomOutButton
        {
            get
            {
                return _zoomOutButton;
            }
            set
            {
                _zoomOutButton = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableButton _zoomResetButton;
        public ObservableButton ZoomResetButton
        {
            get
            {
                return _zoomResetButton;
            }
            set
            {
                _zoomResetButton = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Actions

        private void ResetZoom()
        {
            MapScaling = DefaultZoom;
        }

        private bool IsZoomResetEnabled()
        {
            return Math.Abs(MapScaling - DefaultZoom) > 0.001f;
        }

        private void ZoomIn()
        {
            MapScaling += ZoomChangeStep;
        }

        private bool IsZoomInEnabled()
        {
            return MapScaling <= MaximumAllowedZoom;
        }

        private void ZoomOut()
        {
            MapScaling -= ZoomChangeStep;
        }

        private bool IsZoomOutEnabled()
        {
            return MapScaling >= MinimumAllowedZoom;
        }

        #endregion

        public const float DefaultZoom = 1.0f;
        public const float MinimumAllowedZoom = 0.1f;
        public const float MaximumAllowedZoom = 2.0f;
        public const float ZoomChangeStep = 0.05f;
    }
}
