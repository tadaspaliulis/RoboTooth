using System.Windows;
namespace RoboTooth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new ViewModel.ViewOrchestrator();
            InitializeComponent();
        }
    }
}
