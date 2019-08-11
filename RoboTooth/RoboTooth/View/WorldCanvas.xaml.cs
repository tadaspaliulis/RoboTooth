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
using RoboTooth.ViewModel.Drawing;
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

        public static readonly DependencyProperty CanvasProperty =
        DependencyProperty.Register(
            "Canvas", typeof(CanvasVM), typeof(WorldCanvas)/*,  new PropertyMetadata(OnMovementMapPropertyChanged)*/);

        public CanvasVM Canvas
        {
            get { return (CanvasVM)GetValue(CanvasProperty); }
            set { SetValue(CanvasProperty, value); }
        }

        private void OnPointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DrawMap((ObservableCollection<ViewModel.Line>)sender);
        }

        private static void OnMovementMapPropertyChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            var world = property as WorldCanvas;
            
            if (args.OldValue != null)
            {
                var oldValue = args.OldValue as CanvasVM;
                // Unsubscribe from CollectionChanged on the old collection
                oldValue.Drawables.CollectionChanged -= world.OnPointsCollectionChanged;
            }

            if (args.NewValue != null)
            {
                var newCanvas = args.NewValue as CanvasVM;
                world.Canvas = newCanvas;

                // Subscribe to CollectionChanged on the new collection
                newCanvas.Drawables.CollectionChanged += world.OnPointsCollectionChanged;

                //Do the initial draw for this newly assigned collection
                //world.DrawMap(newCanvas.Drawables);
            }

            Console.WriteLine("Success!");
        }

        private void DrawMap(ObservableCollection<ViewModel.Line> Points)
        {
            /*foreach (var point in Points)
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
            }*/

        }
    }
}
