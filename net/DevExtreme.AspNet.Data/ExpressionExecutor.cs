using DevExtreme.AspNet.Data.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    class ExpressionExecutor {
        IQueryProvider Provider;
        Expression Expr;

        readonly QueryProviderInfo ProviderInfo;
        readonly CancellationToken CancellationToken;
        readonly bool Sync;
        readonly bool AllowAsyncOverSync;

        public ExpressionExecutor(IQueryProvider provider, Expression expr, QueryProviderInfo providerInfo, CancellationToken cancellationToken, bool sync, bool allowAsyncOverSync) {
            Provider = provider;
            Expr = expr;

            ProviderInfo = providerInfo;
            CancellationToken = cancellationToken;
            Sync = sync;
            AllowAsyncOverSync = allowAsyncOverSync;
        }

        public void BreakQueryableChain() {
            var stack = new Stack<MethodCallExpression>();

            while(Expr is MethodCallExpression call && call.Method.DeclaringType == typeof(Queryable)) {
                stack.Push(call);
                Expr = call.Arguments[0];
            }

            while(stack.Count > 0) {
                var origCall = stack.Pop();

                var newCall = Expression.Call(origCall.Method, new List<Expression>(origCall.Arguments) {
                    [0] = Expr
                });

                if(stack.Count > 0) {
                    var query = Provider.CreateQuery(newCall);
                    Provider = query.Provider;
                    Expr = Expression.Constant(query);
                } else {
                    Expr = newCall;
                }
            }
        }

        public Task<IEnumerable<T>> ToEnumerableAsync<T>() {
            CancellationToken.ThrowIfCancellationRequested();
            return CreateAsyncAdapter().ToEnumerableAsync<T>(Provider, Expr, CancellationToken);
        }

        public Task<int> CountAsync() {
            CancellationToken.ThrowIfCancellationRequested();
            return CreateAsyncAdapter().CountAsync(Provider, Expr, CancellationToken);
        }

        IAsyncAdapter CreateAsyncAdapter() {
            if(Sync)
                return AsyncOverSyncAdapter.Instance;

            var providerType = Provider.GetType();

            var customAdapter = CustomAsyncAdapters.GetAdapter(providerType);
            if(customAdapter != null)
                return customAdapter;

            if(ReflectionAsyncAdapter.SupportsProvider(ProviderInfo))
                return new ReflectionAsyncAdapter(ProviderInfo);

            if(AllowAsyncOverSync)
                return AsyncOverSyncAdapter.Instance;

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
