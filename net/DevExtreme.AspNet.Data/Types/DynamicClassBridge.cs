using System;
using System.IO;
using System.Reflection;

namespace DevExtreme.AspNet.Data.Types {

    static class DynamicClassBridge {
        static readonly Type
            FACTORY_TYPE,
            CLASS_TYPE,
            PROP_TYPE;

        static readonly MethodInfo
            CREATE_TYPE_METHOD,
            INDEXER_METHOD;

        static DynamicClassBridge() {
            try {
                var assembly = Assembly.Load("System.Linq.Dynamic.Core");
                FACTORY_TYPE = assembly.GetType("System.Linq.Dynamic.Core.DynamicClassFactory");
                CLASS_TYPE = assembly.GetType("System.Linq.Dynamic.Core.DynamicClass");
                PROP_TYPE = assembly.GetType("System.Linq.Dynamic.Core.DynamicProperty");
                CREATE_TYPE_METHOD = FACTORY_TYPE.GetMethod("CreateType");
                INDEXER_METHOD = CLASS_TYPE.GetMethod("get_Item");
            } catch(FileNotFoundException x) {
                throw new Exception("Please install 'System.Linq.Dynamic.Core' package", x);
            }
        }

        public static Type CreateType(Type[] memberTypes) {
            var props = Array.CreateInstance(PROP_TYPE, memberTypes.Length);
            for(var i = 0; i < memberTypes.Length; i++)
                props.SetValue(Activator.CreateInstance(PROP_TYPE, AnonType.IndexToField(i), memberTypes[i]), i);
            return (Type)CREATE_TYPE_METHOD.Invoke(null, new object[] { props, true });
        }

        public static object GetMember(object obj, int index)
            => INDEXER_METHOD.Invoke(obj, new object[] { AnonType.IndexToField(index) });

#if DEBUG
        public static void ValidateInstance(object obj) {
            if(!CLASS_TYPE.IsAssignableFrom(obj.GetType()))
                throw new ArgumentException();
        }
#endif
    }

}
