using System.ComponentModel;

namespace RoboTooth.ViewModel.Commands.CanExecuteEvalTriggers
{
    public class PropertyChangedCanExecuteTrigger : CanExecuteEvaluationTrigger
    {
        public PropertyChangedCanExecuteTrigger(string watchedPropertyName, INotifyPropertyChanged observableObject)
        {
            _watchedPropertyName = watchedPropertyName;
            observableObject.PropertyChanged += HandlePropertyValueChanged;
        }

        public void HandlePropertyValueChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == _watchedPropertyName)
            {
                InvokeEvent();
            }
        }

        private readonly string _watchedPropertyName;
    }
}
