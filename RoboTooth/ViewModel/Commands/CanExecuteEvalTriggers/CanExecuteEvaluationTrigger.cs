

namespace RoboTooth.ViewModel.Commands.CanExecuteEvalTriggers
{
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
}
