using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Huojian.LibraryManagement.Converters;

public class EnumToVisibilityConverter : IValueConverter
{
    public bool Inverse { get; set; }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null || !(value is Enum))
            return Visibility.Collapsed;

        var currentState = value.ToString();
        var stateStrings = parameter.ToString();
        var found = false;

        foreach (var state in stateStrings.Split(','))
        {
            found = currentState == state.Trim();

            if (found)
                break;
        }
        if (Inverse)
            return found ? Visibility.Collapsed : Visibility.Visible;
        return found ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}