using RoboTooth.ViewModel.Drawing;
using System.Windows;
using System.Windows.Controls;

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

        #region Canvas property
        public static readonly DependencyProperty CanvasProperty =
        DependencyProperty.Register(
            nameof(Canvas), typeof(CanvasVM), typeof(WorldCanvas));

        public CanvasVM Canvas
        {
            get { return (CanvasVM)GetValue(CanvasProperty); }
            set { SetValue(CanvasProperty, value); }
        }
        #endregion
    }
}
