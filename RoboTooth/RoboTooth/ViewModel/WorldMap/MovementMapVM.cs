using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    /// <summary>
    /// Represents a map of movements that the robot has done so far.
    /// </summary>
    public class MovementMapVM : ObservableObject
    {
        private ObservableCollection<LineVM> _points;
        public ObservableCollection<LineVM> Points
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
                NotifyPropertyChanged();
            }
        }
    }
}
