using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace RoboTooth.ViewModel
{
    public interface ICanExecuteChangedInvoker
    {
        void InvokeCanExecuteChanged();
    }

    public class PropertyChangedCanExecuteTrigger : CanExecuteEvaluationTrigger
    {
        public PropertyChangedCanExecuteTrigger(string watchedPropertyName, INotifyPropertyChanged observableObject)
        {
            _watchedPropertyName = watchedPropertyName;
            observableObject.PropertyChanged += HandlePropertyValueChanged;
        }

        public void HandlePropertyValueChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if(propertyChangedEventArgs.PropertyName == _watchedPropertyName)
            {
                InvokeEvent();
            }
        }

        private readonly string _watchedPropertyName;
    }

    public abstract class CanExecuteEvaluationTrigger
    {
        public void Initialise(ICanExecuteChangedInvoker canExecuteChangedEventInvoker)
        {
            _canExecuteChangedEventInvoker = canExecuteChangedEventInvoker;
        }

        /// <summary>
        /// Invokes the CanExecuteChanged event.
        /// </summary>
        protected void InvokeEvent()
        {
            _canExecuteChangedEventInvoker.InvokeCanExecuteChanged();
        }

        private ICanExecuteChangedInvoker _canExecuteChangedEventInvoker;
    }

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
            foreach (EventHandler handler in CanExecuteChanged.GetInvocationList())
                handler.BeginInvoke(this, EventArgs.Empty, null, null);
        }

        public void AddCanExecuteChangedTrigger(CanExecuteEvaluationTrigger canExecuteEvaluationTrigger)
        {
            canExecuteEvaluationTrigger.Initialise(this);
            _canExecuteEvaluationTriggers.Add(canExecuteEvaluationTrigger);
        }

        public void StateChangeHandler(object sender, EventArgs e)
        {
            //THIS IS REALLY TERRIBLE :( need to do something about it
            foreach (EventHandler handler in CanExecuteChanged.GetInvocationList())
                handler.BeginInvoke(sender, e, null, null);
        }

        protected Action<object> _execute;
        private Func<object, bool> _canExecute;
        private List<CanExecuteEvaluationTrigger> _canExecuteEvaluationTriggers = new List<CanExecuteEvaluationTrigger>();
    }
}
