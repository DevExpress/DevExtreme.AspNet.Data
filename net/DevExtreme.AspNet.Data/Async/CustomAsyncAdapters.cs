using System;
using System.Linq;
using System.Collections.Generic;

namespace DevExtreme.AspNet.Data.Async {
    using RegisteredAdapters = List<Tuple<Func<Type, bool>, IAsyncAdapter>>;

    public static class CustomAsyncAdapters {
        static readonly RegisteredAdapters _registeredAdapters = new RegisteredAdapters();

        public static void RegisterAdapter(Func<Type, bool> queryProviderTypePredicate, IAsyncAdapter adapter) {
            _registeredAdapters.Add(Tuple.Create(queryProviderTypePredicate, adapter));
        }

        public static void RegisterAdapter(Type queryProviderType, IAsyncAdapter adapter) {
            RegisterAdapter(type => queryProviderType.IsAssignableFrom(type), adapter);
        }

        internal static IAsyncAdapter GetAdapter(Type queryProviderType) {
            foreach(var i in _registeredAdapters) {
                if(i.Item1(queryProviderType))
                    return i.Item2;
            }
            return null;
        }

#if DEBUG
        internal static void Clear() {
            _registeredAdapters.Clear();
        }
#endif
    }

}
