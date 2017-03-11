using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RoboTooth.ViewModel
{
    /// <summary>
    /// A button class that provides functionality for controlling content, enabled/disabled state and the command executed on click
    /// </summary>
    public class ObservableButton : ObservableObject
    {
        public ObservableButton(ICommand command, object stateObject)
        {
            _buttonCommand = command;
            command.CanExecuteChanged += OnCanExecuteStateChanged;
            Enabled = command.CanExecute(stateObject);
            _stateObject = stateObject;
        }

        private string _content;
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

        private bool _enabled;
        /// <summary>
        /// Enables or disables the control
        /// </summary>
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            private set
            {
                _enabled = value;
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

        private void OnCanExecuteStateChanged(object sender, System.EventArgs e)
        {
            Enabled = _buttonCommand.CanExecute(_stateObject);
        }

        private void OnExecutionStatusChangeChanged(object sender, CommandExecutionStatus status)
        {
            Enabled = _buttonCommand.CanExecute(_stateObject);
        }

        private ICommand _buttonCommand;
        private object _stateObject;
    }
}