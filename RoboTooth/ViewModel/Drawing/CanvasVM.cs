using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace RoboTooth.ViewModel.Drawing
{
    public class CanvasVM : ObservableObject
    {
        public CanvasVM()
        {
            _viewPortSettings = new ViewPortSettings();
        }

        public void AddDrawable(DrawableObservable drawable)
        {
            if (drawable == null)
                throw new ArgumentNullException(nameof(drawable));

            drawable.SetViewPortSettings(ViewSettings);
            Drawables.Add(drawable);
        }

        #region Observable Properties
        private ViewPortSettings _viewPortSettings;

        /// <summary>
        /// Controls the paning and zooming of the canvas
        /// </summary>
        public ViewPortSettings ViewSettings
        {
            get { return _viewPortSettings; }
            set
            {
                _viewPortSettings = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<DrawableObservable> _drawables = new ObservableCollection<DrawableObservable>();
        public ObservableCollection<DrawableObservable> Drawables
        {
            get
            {
                return _drawables;
            }
            set
            {
                _drawables = value;
                NotifyPropertyChanged();
            }
        } 
        #endregion
    }
}
