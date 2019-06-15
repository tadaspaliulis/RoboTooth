using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using RoboTooth.Model.State;

namespace RoboTooth.ViewModel.WorldMap
{
    /// <summary>
    /// A view model of a line.
    /// </summary>
    public class LineVM : ObservableObject
    {
        private float _originX;
        public float OriginX
        {
            get { return _originX; }
            set
            {
                _originX = value;
                NotifyPropertyChanged();
            }
        }

        private float _originY;
        public float OriginY
        {
            get { return _originY; }
            set
            {
                _originY = value;
                NotifyPropertyChanged();
            }
        }

        private float _destinationX;
        public float DestinationX
        {
            get { return _destinationX; }
            set
            {
                _destinationX = value;
                NotifyPropertyChanged();
            }
        }

        private float _destinationY;
        public float DestinationY
        {
            get { return _destinationY; }
            set
            {
                _destinationY = value;
                NotifyPropertyChanged();
            }
        }


        private bool _isPlannedOnly;
        public bool IsPlannedOnly
        {
            get { return _isPlannedOnly; }
            set
            {
                _isPlannedOnly = value;
                NotifyPropertyChanged();
            }
        }

    }

    public class ViewPortSettings : ObservableObject
    {
        private float _mapScaling;
        public float MapScaling
        {
            get { return _mapScaling; }
            set
            {
                _mapScaling = value;
                NotifyPropertyChanged();
            }
        }        
    }

    /// <summary>
    /// Represents a map of movements that the robot has made so far.
    /// </summary>
    public class MovementMapVM : ObservableObject
    {
        public void HandleNewMovementRecordAdded(MovementRecord movementRecord)
        {
            if (Application.Current.Dispatcher != null)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    Lines.Add(new LineVM
                    {
                        OriginX = movementRecord.StartPosition.X + 150,
                        OriginY = movementRecord.StartPosition.Y + 150,
                        DestinationX = movementRecord.Destination.X + 150,
                        DestinationY = movementRecord.Destination.Y + 150,
                        IsPlannedOnly = movementRecord.IsPlannedOnly,
                    });
                });
            }
        }

        public void HandleLastMovementRecordUpdated(MovementRecord movementRecord)
        {
            if (Application.Current?.Dispatcher != null)
            {
                Application.Current?.Dispatcher.Invoke(delegate
                {
                    if(Lines.Count == 0)
                    {
                        //TODO: Is this exception going to be lost since it's in a different thread.
                        throw new InvalidOperationException("Attempted to update a line when the list of lines was empty.");
                    }
                    var lastLine = Lines.Last();

                    lastLine.DestinationX = movementRecord.Destination.X + 150;
                    lastLine.DestinationY = movementRecord.Destination.Y + 150;
                });
            }
        }

        private ObservableCollection<LineVM> _lines;
        public ObservableCollection<LineVM> Lines
        {
            get
            {
                return _lines;
            }
            set
            {
                _lines = value;
                NotifyPropertyChanged();
            }
        }
    }
}
