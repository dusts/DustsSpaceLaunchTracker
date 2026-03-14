using DustsSpaceLaunchTracker.Models;
using System.Globalization;

namespace DustsSpaceLaunchTracker.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not LaunchStatus status || string.IsNullOrEmpty(status?.Abbrev))
                return Colors.Gray;

            return status.Abbrev.ToUpperInvariant() switch
            {
                "GO" or "NET" => Colors.Green,
                "SUCCESS" => Colors.DarkGreen,
                "FAILURE" or "PARTIAL FAILURE" => Colors.Red,
                "IN FLIGHT" or "IN PROGRESS" => Colors.DodgerBlue,
                "TBD" or "TBC" or "TO BE CONFIRMED" => Colors.Orange,
                "HOLD" or "DELAYED" => Colors.DarkOrange,
                "CANCELED" or "SCRUBBED" => Colors.Gray,
                _ => Colors.Gray
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
