using RoboTooth.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace RoboTooth.View.BasicControls
{
    /// <summary>
    /// Interaction logic for BasicButton.xaml
    /// </summary>
    public partial class BasicButton : UserControl
    {
        public BasicButton()
        {
            InitializeComponent();
        }

        #region Button property
        public static readonly DependencyProperty ButtonProperty =
                DependencyProperty.Register(nameof(Button), 
                                            typeof(ObservableButton),
                                            typeof(BasicButton));

        public ObservableButton Button
        {
            get { return (ObservableButton)GetValue(ButtonProperty); }
            set { SetValue(ButtonProperty, value); }
        }
        #endregion

        #region Text property
        public static readonly DependencyProperty TextProperty =
                DependencyProperty.Register(nameof(Text),
                                            typeof(string),
                                            typeof(BasicButton),
                                            new PropertyMetadata(null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion
    }
}
