using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Todo.Conveter
{
    public class TagCornerConverter : IValueConverter
    {
        // Token: 0x06006F93 RID: 28563 RVA: 0x001D9D60 File Offset: 0x001D7F60
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            if (text != null)
            {
                return text;
            }
            return new CornerRadius(0.0);
        }

        // Token: 0x06006F94 RID: 28564 RVA: 0x0004B1A3 File Offset: 0x000493A3
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
