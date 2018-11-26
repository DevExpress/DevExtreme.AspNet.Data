using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace DevExtreme.AspNet.Data {

    static class SelectHelper {

        public static IEnumerable<ExpandoObject> Evaluate<T>(IEnumerable<T> data, IEnumerable<string> select) {
            var bufferedSelect = select.ToArray();
            var paths = SelectToPaths(bufferedSelect);
            var accessor = new DefaultAccessor<T>();

            foreach(var item in data)
                yield return PathsToExpando(paths, i => accessor.Read(item, bufferedSelect[i]));
        }

        public static IEnumerable<ExpandoObject> ConvertRemoteResult(IEnumerable<AnonType> remoteResult, IEnumerable<string> select) {
            var paths = SelectToPaths(select);

            foreach(var anonObj in remoteResult)
                yield return PathsToExpando(paths, i => anonObj[i]);
        }

        static string[][] SelectToPaths(IEnumerable<string> select) {
            return select.Select(i => i.Split('.')).ToArray();
        }

        static ExpandoObject PathsToExpando(string[][] paths, Func<int, object> pathValueByIndex) {
            var expando = new ExpandoObject();

            for(var i = 0; i < paths.Length; i++)
                Shrink(expando, paths[i], pathValueByIndex(i));

            return expando;
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
