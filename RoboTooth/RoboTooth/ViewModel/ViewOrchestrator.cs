using RoboTooth.Model.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoboTooth.Model.MessagingService.Messages;
using System.Collections.ObjectModel;

namespace RoboTooth.ViewModel
{

    public class MessageListItem
    {
        public MessageListItem(RawMessage message)
        {
            _message = message;
        }

        public string RawData
        {
            get
            {
                return _message.GetRawDataAsString();
            }
        }

        public int Id
        {
            get
            {
                return _message.Id;
            }
        }

        private RawMessage _message;
    }

    /// <summary>
    /// ViewOrchestrator controls all the possible views in the application and initialises control objects
    /// </summary>
    class ViewOrchestrator : ObservableObject
    {
        public ViewOrchestrator()
        {   
            InitialiseControllers();

            _connectionManagement = new ConnectionManagementView(_mainController.GetCommunicationInterface());
            _rawMessageList = new ObservableCollection<MessageListItem>();
            _mainController.GetMessageSorter().UnfilteredMessages += HandleReceivedMessages;
        }
            
        private void InitialiseControllers()
        {
            _mainController = new MainController();
        }

        private ObservableCollection<MessageListItem> _rawMessageList;
        public ObservableCollection<MessageListItem> RawMessageList
        {   get
            {
                return _rawMessageList;
            }
            set
            {
                _rawMessageList = value;
                NotifyPropertyChanged();
            }
        }

        private void HandleReceivedMessages(object sender, RawMessage message)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                _rawMessageList.Add(new MessageListItem(message));
            });
            
        }

        private MainController _mainController;

        private ConnectionManagementView _connectionManagement;
        public ConnectionManagementView ConnectionManagement
        {
            get
            {
                return _connectionManagement;
            }
            set
            {
                _connectionManagement = value;
                NotifyPropertyChanged();
            }
        }
    }
}
