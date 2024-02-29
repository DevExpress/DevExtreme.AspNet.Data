using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace DevExtreme.AspNet.Data {

    static class Utils {

        public static bool CanAssignNull(Type type) {
            return !type.GetTypeInfo().IsValueType || IsNullable(type);
        }

        public static bool IsNullable(Type type) {
            var info = type.GetTypeInfo();
            return info.IsGenericType && info.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type MakeNullable(Type type) {
            return typeof(Nullable<>).MakeGenericType(type);
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

            if(type.IsAssignableFrom(value.GetType()))
                return value;

            type = StripNullableType(type);

            if(IsIntegralType(type) && value is String)
                value = Convert.ToDecimal(value, CultureInfo.InvariantCulture);

            if(type == typeof(DateTime) && value is String)
                return DateTime.Parse((string)value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            if(type == typeof(DateTimeOffset) && value is DateTime date)
                return new DateTimeOffset(date);

            var converter = TypeDescriptor.GetConverter(type);
            if(converter != null && converter.CanConvertFrom(value.GetType()))
                return converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);

            if(type.GetTypeInfo().IsEnum)
                return Enum.Parse(type, Convert.ToString(value), true);

            return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        public static Type StripNullableType(Type type) {
            var underlying = Nullable.GetUnderlyingType(type);
            if(underlying != null)
                return underlying;

            return type;
        }

        public static string GetSortMethod(bool first, bool desc) {
            return first
                ? (desc ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy))
                : (desc ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy));
        }

        public static IEnumerable<SortingInfo> AddRequiredSort(IEnumerable<SortingInfo> sort, IEnumerable<string> requiredSelectors) {
            sort = sort ?? new SortingInfo[0];
            requiredSelectors = requiredSelectors.Except(sort.Select(i => i.Selector), StringComparer.OrdinalIgnoreCase);

            var desc = sort.LastOrDefault()?.Desc;

            return sort.Concat(requiredSelectors.Select(i => new SortingInfo {
                Selector = i,
                Desc = desc != null && desc.Value
            }));
        }

        public static string[] GetPrimaryKey(Type type) {
            return new MemberInfo[0]
                .Concat(type.GetRuntimeProperties())
                .Concat(type.GetRuntimeFields())
                .Where(m => m.GetCustomAttributes(true).Any(i => i.GetType().Name == "KeyAttribute"))
                .Select(m => m.Name)
                .OrderBy(i => i)
                .ToArray();
        }

        public static int DynamicCompare(object selectorResult, object clientValue, bool stringToLower) {
            if(selectorResult is DBNull)
                selectorResult = null;

            if(selectorResult != null) {
                clientValue = ConvertClientValue(clientValue, selectorResult.GetType());

                if(stringToLower && clientValue != null) {
                    if(selectorResult is String selectorResultString) {
                        selectorResult = selectorResultString.ToLower();
                    } else if(selectorResult is Char selectorResultChar) {
                        selectorResult = Char.ToLower(selectorResultChar);
                    }
                }
            } else {
                clientValue = UnwrapNewtonsoftValue(clientValue);
            }

            return Comparer<object>.Default.Compare(selectorResult, clientValue);
        }

        public static object UnwrapNewtonsoftValue(object value) {
            if(value != null) {
                var type = value.GetType();
                if(type.FullName.Equals("Newtonsoft.Json.Linq.JValue"))
                    return type.GetProperty("Value").GetValue(value, null);
            }
            return value;
        }

        static bool IsIntegralType(Type type) {
            return type == typeof(int)
                || type == typeof(long)
                || type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(uint)
                || type == typeof(ulong)
                || type == typeof(short)
                || type == typeof(ushort);
        }

    }

    internal static class Compatibility {
        internal static IList UnwrapList(IList deserializedList) {
            var unwrappedList = new List<object>();
            foreach(var item in deserializedList)
                unwrappedList.Add(UnwrapJsonElement(item));
            return unwrappedList;
        }

        static object UnwrapJsonElement(object deserializeObject) {
            if(!(deserializeObject is JsonElement jsonElement))
                return null;

            //https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to#deserialize-inferred-types-to-object-properties

            switch(jsonElement.ValueKind) {
                case JsonValueKind.Array:
                    return jsonElement.EnumerateArray().Select(item => UnwrapJsonElement(item)).ToList();
                case JsonValueKind.String:
                    return jsonElement.GetString();
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.False:
                case JsonValueKind.True:
                    return jsonElement.GetBoolean();
                case JsonValueKind.Number:
                    //same as IsIntegralType + unsigned?
                    if(jsonElement.TryGetInt32(out var intValue))
                        return intValue;
                    if(jsonElement.TryGetInt64(out var longValue))
                        return longValue;
                    if(jsonElement.TryGetSByte(out var sByteValue))
                        return sByteValue;
                    if(jsonElement.TryGetInt16(out var shortValue))
                        return shortValue;
                    //or floating point as well?
                    if(jsonElement.TryGetDouble(out var doubleValue))
                        return doubleValue;
                    if(jsonElement.TryGetDecimal(out var decimalValue))
                        return decimalValue;
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
    }

}
