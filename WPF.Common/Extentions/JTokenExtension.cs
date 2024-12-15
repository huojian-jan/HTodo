using Newtonsoft.Json.Linq;

namespace WPF.Common.Extentions
{
    public static class JTokenExtension
    {
        public static T Resolve<T>(this JToken jValue)
        {
            var type = typeof(T);
            if (type.IsEnum)
            {
                if (jValue == null)
                    return default;
                var value = jValue.ToString();
                return (T)Enum.Parse(type, value, true);
            }
            else
            {
                if (jValue == null)
                    return default;
                return jValue.ToObject<T>();
                //return jValue.Value<T>();
            }
        }

        public static object Resolve(this JToken jValue, Type type)
        {
            if (type.IsEnum)
            {
                if (jValue == null)
                    return 0;
                var value = jValue.ToString();
                return Enum.Parse(type, value, true);
            }
            else
            {
                if (jValue == null)
                    return type.IsValueType ? Activator.CreateInstance(type) : null;
                return jValue.ToObject(type);
            }
        }

        public static T Resolve<T>(this JToken jToken, string key, T defaultValue = default)
        {
            var type = typeof(T);
            if (type.IsEnum)
            {
                var jValue = jToken[key];
                if (jValue == null)
                    return defaultValue;
                var value = jValue.ToString();
                return (T)Enum.Parse(type, value, true);
            }
            else
            {
                var jValue = jToken[key];
                if (jValue == null)
                    return defaultValue;
                return jValue.ToObject<T>();
            }
        }

        public static object Resolve(this JToken jToken, string key, Type type)
        {
            if (type.IsEnum)
            {
                var jValue = jToken[key];
                if (jValue == null)
                    return 0;
                var value = jValue.ToString();
                return Enum.Parse(type, value, true);
            }
            else
            {
                var jValue = jToken[key];
                if (jValue == null)
                    return type.IsValueType ? Activator.CreateInstance(type) : null;
                return jValue.ToObject(type);
            }
        }
    }
}
