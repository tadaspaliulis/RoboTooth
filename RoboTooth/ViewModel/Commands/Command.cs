using RoboTooth.ViewModel.Commands.CanExecuteEvalTriggers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace RoboTooth.ViewModel.Commands
{
    internal class Command : ICommand, ICanExecuteChangedInvoker
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

        public void InvokeCanExecuteChanged()
        {
            Application.Current?.Dispatcher.Invoke(delegate
            {
                CanExecuteChanged(this, EventArgs.Empty);
            });
        }

        public void AddCanExecuteChangedTrigger(CanExecuteEvaluationTrigger canExecuteEvaluationTrigger)
        {
            canExecuteEvaluationTrigger.Initialise(this);
            _canExecuteEvaluationTriggers.Add(canExecuteEvaluationTrigger);
        }

        protected Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        private readonly List<CanExecuteEvaluationTrigger> _canExecuteEvaluationTriggers = new List<CanExecuteEvaluationTrigger>();
    }
}
