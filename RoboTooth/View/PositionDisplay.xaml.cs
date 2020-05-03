using RoboTooth.ViewModel.DataDisplayVM;
using System.Windows;
using System.Windows.Controls;

namespace RoboTooth.View
{
    /// <summary>
    /// Interaction logic for PositionDisplay.xaml
    /// </summary>
    public partial class PositionDisplay : UserControl
    {
        public PositionDisplay()
        {
            InitializeComponent();
        }

        #region Internal Data Property property
        public static readonly DependencyProperty InternalDataProperty =
                DependencyProperty.Register(nameof(InternalData),
                                            typeof(InternalDataDisplay),
                                            typeof(PositionDisplay),
                                            new PropertyMetadata(null));

        public InternalDataDisplay InternalData
        {
            get { return (InternalDataDisplay)GetValue(InternalDataProperty); }
            set { SetValue(InternalDataProperty, value); }
        }
        #endregion
    }
}
