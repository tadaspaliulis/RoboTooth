using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace RoboTooth.View.Converters
{
    /// <summary>
    /// Converts multi binding to the first non null value
    /// </summary>
    public class RankedPreferenceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, 
                              Type targetType, 
                              object parameter,
                              CultureInfo culture)
        {
            if (values == null)
                return null;

            // Return the first non-null value
            return values.FirstOrDefault(v => v != null);
        }

        public object[] ConvertBack(object value, 
                                    Type[] targetTypes, 
                                    object parameter, 
                                    CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
