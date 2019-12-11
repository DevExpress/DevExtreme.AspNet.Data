using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Async {

    class AsyncHelper {
        readonly IQueryProvider Provider;
        readonly CancellationToken CancellationToken;
        readonly Lazy<IAsyncAdapter> Adapter;

        public AsyncHelper(IQueryProvider provider, QueryProviderInfo providerInfo, bool allowAsyncOverSync, CancellationToken cancellationToken) {
            Provider = provider;
            CancellationToken = cancellationToken;

            IAsyncAdapter CreateAdapter() {
                var providerType = Provider.GetType();

                var customAdapter = CustomAsyncAdapters.GetAdapter(providerType);
                if(customAdapter != null)
                    return customAdapter;

                var reflectionAdapter = new ReflectionAsyncAdapter(providerInfo);
                if(reflectionAdapter.IsSupportedProvider)
                    return reflectionAdapter;

                if(allowAsyncOverSync)
                    return AsyncOverSyncAdapter.Instance;

                throw ProviderNotSupported(providerType);
            }

            Adapter = new Lazy<IAsyncAdapter>(CreateAdapter);
        }

        public Task<int> CountAsync(Expression expr) {
            CancellationToken.ThrowIfCancellationRequested();
            return Adapter.Value.CountAsync(Provider, expr, CancellationToken);
        }

        public Task<IEnumerable<T>> ToEnumerableAsync<T>(Expression expr) {
            CancellationToken.ThrowIfCancellationRequested();
            return Adapter.Value.ToEnumerableAsync<T>(Provider, expr, CancellationToken);
        }

        static Exception ProviderNotSupported(Type providerType) {
            if(providerType.IsGenericType)
                providerType = providerType.GetGenericTypeDefinition();

            var message = $"Async operations for the LINQ provider '{providerType.FullName}' are not supported."
                + $" You can implement a custom async adapter ({typeof(IAsyncAdapter).FullName}) and register it via '{typeof(CustomAsyncAdapters).FullName}.{nameof(CustomAsyncAdapters.RegisterAdapter)}'.";

            return new NotSupportedException(message);
        }

    }

}
