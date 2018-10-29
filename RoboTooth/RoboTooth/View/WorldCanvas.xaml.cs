using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RoboTooth.ViewModel.WorldMap;

namespace RoboTooth.View
{
    /// <summary>
    /// Interaction logic for WorldCanvas.xaml
    /// </summary>
    public partial class WorldCanvas : UserControl
    {
        public WorldCanvas()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MovementMapProperty =
        DependencyProperty.Register(
            "MovementMap", typeof(MovementMapVM), typeof(WorldCanvas)/*,  new PropertyMetadata(OnMovementMapPropertyChanged)*/);

        public MovementMapVM MovementMap
        {
            get { return (MovementMapVM)GetValue(MovementMapProperty); }
            set { SetValue(MovementMapProperty, value); }
        }

        private void OnPointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DrawMap((ObservableCollection<LineVM>)sender);
        }

        private static void OnMovementMapPropertyChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var world = property as WorldCanvas;
            
            
            if (args.OldValue != null)
            {
                var oldValue = args.OldValue as MovementMapVM;
                // Unsubscribe from CollectionChanged on the old collection
                oldValue.Lines.CollectionChanged -= world.OnPointsCollectionChanged;
            }

            if (args.NewValue != null)
            {
                var newMovementMap = args.NewValue as MovementMapVM;
                world.MovementMap = newMovementMap;

                // Subscribe to CollectionChanged on the new collection
                newMovementMap.Lines.CollectionChanged += world.OnPointsCollectionChanged;

                //Do the initial draw for this newly assigned collection
                world.DrawMap(newMovementMap.Lines);
            }


            Console.WriteLine("Success!");
        }

        private void DrawMap(ObservableCollection<LineVM> Points)
        {
            foreach (var point in Points)
            {
                var line = new Line
                {
                    X1 = point.OriginX,
                    Y1 = point.OriginY,
                    X2 = point.DestinationX,
                    Y2 = point.DestinationY,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 3,
                };
                //canvasArea.Children.Add(line);
            }

        }
    }
}
