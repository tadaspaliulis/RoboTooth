using RoboTooth.Model.State;
using RoboTooth.ViewModel.Drawing;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RoboTooth.ViewModel.WorldMap
{
    /// <summary>
    /// Represents a map of movements that the robot has made so far.
    /// </summary>
    public class MovementMapVM : ObservableObject
    {
        public MovementMapVM(CanvasVM canvas)
        {
            _canvas = canvas;
        }

        public void HandleNewMovementRecordAdded(MovementRecord movementRecord)
        {
            Application.Current?.Dispatcher?.Invoke(delegate
            {
                var line = new Line
                {
                    StartPointX = movementRecord.StartPosition.X,
                    StartPointY = movementRecord.StartPosition.Y,
                    EndPointX = movementRecord.Destination.X,
                    EndPointY = movementRecord.Destination.Y,
                    //IsPlannedOnly = movementRecord.IsPlannedOnly,
                };

                Lines.Add(line);
                _canvas.AddDrawable(line);
            });
        }

        public void HandleLastMovementRecordUpdated(MovementRecord movementRecord)
        {
            Application.Current?.Dispatcher?.Invoke(delegate
            {
                if (Lines.Count == 0)
                {
                    //TODO: Is this exception going to be lost since it's in a different thread.
                    throw new InvalidOperationException("Attempted to update a line when the list of lines was empty.");
                }

                var lastLine = Lines.Last();

                lastLine.EndPointX = movementRecord.Destination.X;
                lastLine.EndPointY = movementRecord.Destination.Y;
            });
        }

        private ObservableCollection<Line> _lines = new ObservableCollection<Line>();
        public ObservableCollection<Line> Lines
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

        private readonly CanvasVM _canvas;
    }
}
