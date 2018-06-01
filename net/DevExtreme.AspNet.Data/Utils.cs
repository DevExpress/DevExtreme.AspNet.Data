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

            type = StripNullableType(type);

            if(IsIntegralType(type) && value is String)
                value = Convert.ToDecimal(value, CultureInfo.InvariantCulture);

            if(type == typeof(DateTime) && value is String)
                return DateTime.Parse((string)value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

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

        public static int DynamicCompare(object selectorResult, object clientValue) {
            if(selectorResult != null)
                clientValue = ConvertClientValue(clientValue, selectorResult.GetType());
            else
                clientValue = UnwrapNewtonsoftValue(clientValue);

            return Comparer<object>.Default.Compare(selectorResult, clientValue);
        }

        public static object UnwrapNewtonsoftValue(object value) {
            if(value is JValue jValue)
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
