using System.ComponentModel;

namespace RoboTooth.ViewModel.Drawing
{
    /// <summary>
    /// Abstract class for objects that can be drawn and can react
    /// to ViewPort changes.
    /// </summary>
    public abstract class DrawableObservable : ObservableObject
    {
        public abstract void HandleViewPortChange(object sender, PropertyChangedEventArgs e);
        
        public virtual void SetViewPortSettings(ViewPortSettings viewPortSettings)
        {
            viewPortSettings.PropertyChanged += HandleViewPortChange;
            CurrentViewPortSettings = viewPortSettings;
        }

        public IViewPortSettingsReadonly CurrentViewPortSettings { get; private set; }
    }
}
