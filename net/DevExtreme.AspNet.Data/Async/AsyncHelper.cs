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

        AsyncAdapter _adapter;

        public AsyncHelper(IQueryProvider provider, QueryProviderInfo providerInfo, CancellationToken cancellationToken) {
            Provider = provider;
            ProviderInfo = providerInfo;
            CancellationToken = cancellationToken;
        }

        AsyncAdapter Adapter {
            get {
                if(_adapter == null)
                    _adapter = CreateAdapter();
                return _adapter;
            }
        }

        public Task<int> CountAsync(Expression expr) {
            CancellationToken.ThrowIfCancellationRequested();
            return Adapter.CountAsync(expr);
        }

        public Task<IEnumerable<T>> ToEnumerableAsync<T>(Expression expr) {
            CancellationToken.ThrowIfCancellationRequested();
            return Adapter.ToEnumerableAsync<T>(expr);
        }

        AsyncAdapter CreateAdapter() {
            if(ProviderInfo.IsEFClassic)
                return new EFClassicAsyncAdapter(Provider, CancellationToken);

            if(ProviderInfo.IsEFCore && ProviderInfo.Version.Major >= 2)
                return new EFCore2AsyncAdapter(Provider, CancellationToken);

            if(ProviderInfo.IsNH)
                return new NHAsyncAdapter(Provider, CancellationToken);

#warning TODO message
            throw new NotSupportedException();
        }

    }

}
