using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Async {

    class AsyncHelper {
        readonly IQueryProvider Provider;
        readonly QueryProviderInfo ProviderInfo;
        readonly CancellationToken CancellationToken;

        public AsyncHelper(IQueryProvider provider, QueryProviderInfo providerInfo, CancellationToken cancellationToken) {
            Provider = provider;
            ProviderInfo = providerInfo;
            CancellationToken = cancellationToken;
        }

        public Task<int> CountAsync(Expression expr) {
            CancellationToken.ThrowIfCancellationRequested();
            return CreateAdapter().CountAsync(Provider, expr, CancellationToken);
        }

        public Task<IEnumerable<T>> ToEnumerableAsync<T>(Expression expr) {
            CancellationToken.ThrowIfCancellationRequested();
            return CreateAdapter().ToEnumerableAsync<T>(Provider, expr, CancellationToken);
        }

        IAsyncAdapter CreateAdapter() {
            var providerType = Provider.GetType();

            var customAdapter = CustomAsyncAdapters.GetAdapter(providerType);
            if(customAdapter != null)
                return customAdapter;

            var reflectionAdapter = new ReflectionAsyncAdapter(ProviderInfo);
            if(reflectionAdapter.IsSupportedProvider)
                return reflectionAdapter;

            throw ProviderNotSupported(providerType);
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
