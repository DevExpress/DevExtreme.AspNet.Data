using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Async {

    class AsyncOverSyncAdapter : IAsyncAdapter {
        public static readonly IAsyncAdapter Instance = new AsyncOverSyncAdapter();

        private AsyncOverSyncAdapter() {
        }

        // NOTE on Task.FromResult vs. Task.Run https://stackoverflow.com/a/34005461

        public static Task<int> CountAsync(IQueryProvider queryProvider, Expression expr)
            => Task.FromResult(Count(queryProvider, expr));

        public static Task<IEnumerable<T>> ToEnumerableAsync<T>(IQueryProvider queryProvider, Expression expr)
            => Task.FromResult(ToEnumerable<T>(queryProvider, expr));

        public static int Count(IQueryProvider queryProvider, Expression expr)
            => queryProvider.Execute<int>(expr);

        public static IEnumerable<T> ToEnumerable<T>(IQueryProvider queryProvider, Expression expr)
            => queryProvider.CreateQuery<T>(expr);

        Task<int> IAsyncAdapter.CountAsync(IQueryProvider queryProvider, Expression expr, CancellationToken _)
            => CountAsync(queryProvider, expr);

        Task<IEnumerable<T>> IAsyncAdapter.ToEnumerableAsync<T>(IQueryProvider queryProvider, Expression expr, CancellationToken _)
            => ToEnumerableAsync<T>(queryProvider, expr);
    }

}
