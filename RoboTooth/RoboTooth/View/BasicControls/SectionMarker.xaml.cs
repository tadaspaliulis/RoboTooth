using System.Windows;
using System.Windows.Controls;

namespace RoboTooth.View.BasicControls
{
    /// <summary>
    /// Interaction logic for SectionMarker.xaml
    /// </summary>
    public partial class SectionMarker : UserControl
    {
        public SectionMarker()
        {
            InitializeComponent();
        }

        #region Text property
        public static readonly DependencyProperty TextProperty =
                DependencyProperty.Register(nameof(Text),
                                            typeof(string),
                                            typeof(SectionMarker),
                                            new PropertyMetadata(null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion
    }
}
