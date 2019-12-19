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

        readonly CancellationToken CancellationToken;
        readonly bool Sync;

        public ExpressionExecutor(IQueryProvider provider, Expression expr, CancellationToken cancellationToken, bool sync) {
            Provider = provider;
            Expr = expr;
            Sync = sync;
            CancellationToken = cancellationToken;
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
            if(!Sync)
                return CreateAsyncHelper().ToEnumerableAsync<T>(Expr);

            return Task.FromResult((IEnumerable<T>)Provider.CreateQuery<T>(Expr));
        }

        public Task<int> CountAsync() {
            if(!Sync)
                return CreateAsyncHelper().CountAsync(Expr);

            return Task.FromResult(Provider.Execute<int>(Expr));
        }

        AsyncHelper CreateAsyncHelper() => new AsyncHelper(Provider, new QueryProviderInfo(Provider), CancellationToken);
    }

}
