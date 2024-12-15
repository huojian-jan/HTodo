using System.Globalization;
using System.Windows.Data;

namespace Todo.Asset
{
    public class ImageConverter:IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var ass = this.GetType().Assembly.GetManifestResourceNames();
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return default;
        }
    }
}
