using System;
using System.Collections.Generic;
using System.Text;

namespace DustsSpaceLaunchTracker.Converters
{
    public class IsNotNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string str)
                return !string.IsNullOrEmpty(str);

            return value != null;  // fallback for non-string (e.g. object, collection count > 0 later)
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new NotImplementedException();
    }
}
