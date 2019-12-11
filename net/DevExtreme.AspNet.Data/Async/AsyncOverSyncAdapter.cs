using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Async {

    class AsyncOverSyncAdapter : IAsyncAdapter {
        public static readonly AsyncOverSyncAdapter Instance = new AsyncOverSyncAdapter();

        private AsyncOverSyncAdapter() {
        }

        // NOTE on Task.FromResult vs. Task.Run https://stackoverflow.com/a/34005461

        public Task<int> CountAsync(IQueryProvider queryProvider, Expression expr)
            => Task.FromResult(queryProvider.Execute<int>(expr));

        public Task<IEnumerable<T>> ToEnumerableAsync<T>(IQueryProvider queryProvider, Expression expr)
            => Task.FromResult((IEnumerable<T>)queryProvider.CreateQuery<T>(expr));

        Task<int> IAsyncAdapter.CountAsync(IQueryProvider queryProvider, Expression expr, CancellationToken _)
            => CountAsync(queryProvider, expr);

        Task<IEnumerable<T>> IAsyncAdapter.ToEnumerableAsync<T>(IQueryProvider queryProvider, Expression expr, CancellationToken _)
            => ToEnumerableAsync<T>(queryProvider, expr);
    }

}
