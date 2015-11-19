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
            return !type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(type) != null;
        }

        public static object ConvertClientValue(object value, Type type) {
            value = UnwrapNewtonsoftValue(value);

            if(value == null || type == null)
                return value;
            
            type = StripNullableType(type);

            var converter = TypeDescriptor.GetConverter(type);
            if(converter != null && converter.CanConvertFrom(value.GetType()))
                return converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);

            return value;
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

    }

}
