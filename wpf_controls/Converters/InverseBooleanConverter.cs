using System.Globalization;
using System.Windows.Data;

namespace ControlToolKits.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var booleanValue = ConverterHelper.GetBooleanValue(value);
            return !booleanValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var booleanValue = ConverterHelper.GetBooleanValue(value);
            return !booleanValue;
        }
    }
}
