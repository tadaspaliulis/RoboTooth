using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
