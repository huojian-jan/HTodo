using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF.Common.Extentions
{
    public static class FrameworkElementExtension
    {
        public static bool ApplySettings(this FrameworkElement target, IEnumerable<KeyValuePair<string, object>> settings)
        {
            if (settings != null)
            {
                var type = target.GetType();

                foreach (var pair in settings)
                {
                    var propertyInfo = type.GetProperty(pair.Key);

                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(target, pair.Value, null);
                    }
                }

                return true;
            }
            return false;
        }
    }
}
