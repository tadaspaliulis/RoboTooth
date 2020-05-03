using RoboTooth.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RoboTooth.View
{
    /// <summary>
    /// Interaction logic for RawMessageList.xaml
    /// </summary>
    public partial class RawMessageList : UserControl
    {
        public RawMessageList()
        {
            InitializeComponent();
        }

        #region Messages List property
        public static readonly DependencyProperty MessagesListProperty =
                DependencyProperty.Register(nameof(MessagesList),
                                            typeof(ObservableCollection<MessageListItem>),
                                            typeof(RawMessageList),
                                            new PropertyMetadata(null));

        public ObservableCollection<MessageListItem> MessagesList
        {
            get { return (ObservableCollection<MessageListItem>)GetValue(MessagesListProperty); }
            set { SetValue(MessagesListProperty, value); }
        }
        #endregion
    }
}
