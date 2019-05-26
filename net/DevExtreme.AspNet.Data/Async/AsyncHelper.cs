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
            return new ReflectionAsyncAdapter(ProviderInfo);
        }

    }

}
