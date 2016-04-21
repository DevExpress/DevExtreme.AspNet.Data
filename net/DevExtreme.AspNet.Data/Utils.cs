using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace DevExtreme.AspNet.Data {

    static class Utils {

        public static bool CanAssignNull(Type type) {
            return !type.GetTypeInfo().IsValueType || IsNullable(type);
        }

        public static bool IsNullable(Type type) {
            var info = type.GetTypeInfo();
            return info.IsGenericType && info.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static object GetDefaultValue(Type type) {
            if(type.GetTypeInfo().IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }

        public static object ConvertClientValue(object value, Type type) {
            value = UnwrapNewtonsoftValue(value);

            if(value == null || type == null)
                return value;
            
            type = StripNullableType(type);

            if(IsIntegralType(type) && value is String)
                value = Convert.ToDecimal(value, CultureInfo.InvariantCulture);

            var converter = TypeDescriptor.GetConverter(type);
            if(converter != null && converter.CanConvertFrom(value.GetType()))
                return converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);

            return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        public static Type StripNullableType(Type type) {
            var underlying = Nullable.GetUnderlyingType(type);
            if(underlying != null)
                return underlying;

            return type;
        }

        static object UnwrapNewtonsoftValue(object value) {
            var jValue = value as JValue;
            if(jValue != null)
                return jValue.Value;

            return value;
        }

        static bool IsIntegralType(Type type) {
            return type == typeof(int)
                || type == typeof(long)
                || type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(uint)
                || type == typeof(ulong);
        }

    }

}
