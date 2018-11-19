using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace DevExtreme.AspNet.Data {

    static class SelectHelper {

        public static IEnumerable<IDictionary<string, object>> Evaluate<T>(IEnumerable<T> data, IEnumerable<string> names) {
            var bufferedNames = names.ToArray();
            var accessor = new DefaultAccessor<T>();

            foreach(var item in data) {
                IDictionary<string, object> expando = new ExpandoObject();

                foreach(var name in bufferedNames)
                    expando[name] = accessor.Read(item, name);

                yield return expando;
            }
        }

        public static IEnumerable<IDictionary<string, object>> ConvertRemoteResult(IEnumerable<AnonType> selectResult, IEnumerable<string> names) {
            var paths = names.Select(n => n.Split('.')).ToArray();

            foreach(var anonObj in selectResult) {
                var expando = new ExpandoObject();

                for(var i = 0; i < paths.Length; i++)
                    Shrink(expando, paths[i], anonObj[i]);

                yield return expando;
            }
        }

        static void Shrink(IDictionary<string, object> target, string[] path, object value, int index = 0) {
            var key = path[index];

            if(index == path.Length - 1) {
                target[key] = value;
            } else {
                if(!target.ContainsKey(key))
                    target[key] = new ExpandoObject();

                if(target[key] is IDictionary<string, object> child)
                    Shrink(child, path, value, 1 + index);
            }
        }

    }

}
