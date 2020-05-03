using System.Windows.Input;

namespace RoboTooth.ViewModel
{
    /// <summary>
    /// A button class that provides functionality for controlling content,
    /// enabled/disabled state and the command executed on click.
    /// </summary>
    public class ObservableButton : ObservableObject
    {
        public ObservableButton(ICommand command, object stateObject)
        {
            _buttonCommand = command;
            _stateObject = stateObject;
        }

        private string _content;
        /// <summary>
        /// The button text
        /// </summary>
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Command invoked when the button is clicked
        /// </summary>
        public ICommand Command
        {
            get
            {
                return _buttonCommand;
            }
        }

        private readonly ICommand _buttonCommand;
        private readonly object _stateObject;
    }
}