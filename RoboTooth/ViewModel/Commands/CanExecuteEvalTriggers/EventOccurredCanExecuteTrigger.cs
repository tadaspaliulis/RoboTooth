using System;

namespace RoboTooth.ViewModel.Commands.CanExecuteEvalTriggers
{
    /// <summary>
    /// CanExecuteChanged event trigger that listens to an EventHandler type
    /// event
    /// </summary>
    public class EventOccurredCanExecuteTrigger : CanExecuteEvaluationTrigger
    {
        public void HandleEventReceived(object sender, EventArgs propertyChangedEventArgs)
        {
            InvokeEvent();
        }
    }
}
