using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ControlToolKits.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public bool HiddenInsteadOfCollapsed { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanValue = ConverterHelper.GetBooleanValue(value);
            return ConverterHelper.BooleanToVisibility(booleanValue ^ Inverse, HiddenInsteadOfCollapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (value is Visibility && (Visibility)value == Visibility.Visible) ^ Inverse;
            if (targetType == typeof(DefaultBoolean))
            {
                return ConverterHelper.ToDefaultBoolean(flag);
            }
            return flag;
        }
    }
}
