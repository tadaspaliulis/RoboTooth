using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace RoboTooth.ViewModel
{
    internal enum CommandExecutionStatus
    {
        EStartedExecution,
        EExecutionFailed,
    }

    internal class Command : ICommand
    {
        public Command(Func<object, bool> CanExecute, Action<object> Execute)
        {
            _canExecute = CanExecute;
            _execute = Execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;

            return _canExecute(parameter);
        }

        public virtual void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void StateChangeHandler(object sender, System.EventArgs e)
        {
            //THIS IS REALLY TERRIBLE :( need to do something about it
            foreach (EventHandler handler in CanExecuteChanged.GetInvocationList())
                handler.BeginInvoke(sender, e, null, null);
        }

        protected Action<object> _execute;
        private Func<object, bool> _canExecute;
    }
}
