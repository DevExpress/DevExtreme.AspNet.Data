#if NET40
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DevExtreme.AspNet.Data {

    // Reference sources:
    // https://github.com/dotnet/coreclr/blob/release/1.0.0-rc1/src/mscorlib/src/System/Reflection/RuntimeReflectionExtensions.cs
    // https://github.com/dotnet/coreclr/blob/release/1.0.0-rc1/src/mscorlib/src/System/Reflection/TypeInfo.cs

    static class TypeExtensions {
        internal const BindingFlags
            EVERYTHING = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        public static TypeInfo GetTypeInfo(this Type type) {
            return new TypeInfo(type);
        }

        public static IEnumerable<FieldInfo> GetRuntimeFields(this Type type) {
            return type.GetFields(EVERYTHING);
        }

        public static IEnumerable<PropertyInfo> GetRuntimeProperties(this Type type) {
            return type.GetProperties(EVERYTHING);
        }

    }

    class TypeInfo {
        Type _type;

        public TypeInfo(Type type) {
            _type = type;
        }

        public bool IsValueType {
            get { return _type.IsValueType; }
        }

        public bool IsGenericType {
            get { return _type.IsGenericType; }
        }

        public bool IsEnum {
            get { return _type.IsEnum; }
        }

        public Type GetGenericTypeDefinition() {
            return _type.GetGenericTypeDefinition();
        }

    }

}
#endif
