using System.Windows;
using System.Windows.Controls;

namespace RoboTooth.View.BasicControls
{
    /// <summary>
    /// Interaction logic for LabelledTextBox.xaml
    /// </summary>
    public partial class LabelledTextBox : UserControl
    {
        public LabelledTextBox()
        {
            InitializeComponent();
        }

        #region Label Text Property property
        public static readonly DependencyProperty LabelTextProperty =
                DependencyProperty.Register(nameof(LabelText),
                                            typeof(string),
                                            typeof(LabelledTextBox),
                                            new PropertyMetadata(null));

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }
        #endregion

        #region TextBox Content property
        public static readonly DependencyProperty TextBoxContentProperty =
                DependencyProperty.Register(nameof(TextBoxContent),
                                            typeof(string),
                                            typeof(LabelledTextBox),
                                            new PropertyMetadata(null));

        public string TextBoxContent
        {
            get { return (string)GetValue(TextBoxContentProperty); }
            set { SetValue(TextBoxContentProperty, value); }
        }
        #endregion

        #region Read Only  property
        public static readonly DependencyProperty IsReadOnlyProperty =
                DependencyProperty.Register(nameof(IsReadOnly),
                                            typeof(bool),
                                            typeof(LabelledTextBox),
                                            new UIPropertyMetadata(false));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        #endregion

        #region TextBox Width  property
        public static readonly DependencyProperty TextBoxWidthProperty =
                DependencyProperty.Register(nameof(TextBoxWidth),
                                            typeof(int),
                                            typeof(LabelledTextBox),
                                            new UIPropertyMetadata(20));

        public int TextBoxWidth
        {
            get { return (int)GetValue(TextBoxWidthProperty); }
            set { SetValue(TextBoxWidthProperty, value); }
        }
        #endregion
    }
}
