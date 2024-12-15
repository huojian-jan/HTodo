using System.Windows;

namespace ControlToolKits.Converters
{
    internal static class ConverterHelper
    {
        public static string[] GetParameters(object parameter)
        {
            string text = parameter as string;
            if (string.IsNullOrEmpty(text))
            {
                return new string[0];
            }
            return text.Split(';');
        }

        public static bool GetBooleanParameter(string[] parameters, string name)
        {
            foreach (string a in parameters)
            {
                if (string.Equals(a, name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool GetBooleanValue(object value)
        {
            if (value is bool)
            {
                return (bool)value;
            }
            if (value is bool?)
            {
                bool? flag = (bool?)value;
                if (!flag.HasValue)
                {
                    return false;
                }
                return flag.Value;
            }
            if (value is DefaultBoolean)
            {
                return (DefaultBoolean)value == DefaultBoolean.True;
            }
            return false;
        }

        public static bool? GetNullableBooleanValue(object value)
        {
            if (value is bool)
            {
                return (bool)value;
            }
            if (value is bool?)
            {
                return (bool?)value;
            }
            if (value is DefaultBoolean)
            {
                DefaultBoolean defaultBoolean = (DefaultBoolean)value;
                if (defaultBoolean != DefaultBoolean.Default)
                {
                    return defaultBoolean == DefaultBoolean.True;
                }
                return null;
            }
            return null;
        }

        public static DefaultBoolean ToDefaultBoolean(bool? booleanValue)
        {
            if (booleanValue.HasValue)
            {
                if (!booleanValue.Value)
                {
                    return DefaultBoolean.False;
                }
                return DefaultBoolean.True;
            }
            return DefaultBoolean.Default;
        }

        public static bool NumericToBoolean(object value, bool inverse)
        {
            if (value == null)
            {
                return CorrectBoolean(value: false, inverse);
            }
            try
            {
                double num = (double)Convert.ChangeType(value, typeof(double), null);
                return CorrectBoolean(num != 0.0, inverse);
            }
            catch (Exception)
            {
            }
            return CorrectBoolean(value: false, inverse);
        }

        public static bool StringToBoolean(object value, bool inverse)
        {
            if (!(value is string))
            {
                return CorrectBoolean(value: false, inverse);
            }
            return CorrectBoolean(!string.IsNullOrEmpty((string)value), inverse);
        }

        public static Visibility BooleanToVisibility(bool booleanValue, bool hiddenInsteadOfCollapsed)
        {
            if (!booleanValue)
            {
                if (!hiddenInsteadOfCollapsed)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Hidden;
            }
            return Visibility.Visible;
        }

        private static bool CorrectBoolean(bool value, bool inverse)
        {
            return value ^ inverse;
        }
    }


    public enum DefaultBoolean
    {
        /// <summary>
        ///   <para>Corresponds to a Boolean value of true.</para>
        /// </summary>
        True,
        /// <summary>
        ///   <para>Corresponds to a Boolean value of false.</para>
        /// </summary>
        False,
        /// <summary>
        ///   <para>The value is determined by the current object's parent object setting (e.g., a control setting).</para>
        /// </summary>
        Default
    }
}
