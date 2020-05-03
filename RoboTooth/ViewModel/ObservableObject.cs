using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RoboTooth.ViewModel
{
    public class ObservableObject : INotifyPropertyChanged
    {
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
