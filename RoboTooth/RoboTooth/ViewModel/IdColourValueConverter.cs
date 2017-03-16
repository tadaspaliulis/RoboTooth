using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using RoboTooth.Model.MessagingService.Messages;
using System.Windows.Media;

namespace RoboTooth.ViewModel
{
    public class IdColourValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int))
                throw new ArgumentException("value type is not int", "value");

            var Id = (int)value;
            Color c;
            switch((RxMessageIdsEnum)Id)
            {
                case RxMessageIdsEnum.EEchoDistance:
                    c = Colors.Peru;
                    break;
                case RxMessageIdsEnum.EMagnetometerOrientation:
                    c = Colors.LavenderBlush;
                    break;
                default:
                    c = Colors.White;
                    break;
            }

            return new SolidColorBrush(c);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
