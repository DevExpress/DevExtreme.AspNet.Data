using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Async {

    abstract class AsyncAdapter {
        protected readonly IQueryProvider Provider;
        protected readonly CancellationToken CancellationToken;

        public AsyncAdapter(IQueryProvider provider, CancellationToken cancellationToken) {
            Provider = provider;
            CancellationToken = cancellationToken;
        }

        public abstract Task<int> CountAsync(Expression expr);
        public abstract Task<IEnumerable<T>> ToEnumerableAsync<T>(Expression expr);

        protected static MethodInfo FindCountAsyncMethod(Type extensionsType) {
            return FindQueryExtensionMethod(extensionsType, "CountAsync");
        }

        protected static MethodInfo FindToArrayAsyncMethod(Type extensionsType) {
            return FindQueryExtensionMethod(extensionsType, "ToArrayAsync");
        }

        protected static MethodInfo FindToListAsyncMethod(Type extensionsType) {
            return FindQueryExtensionMethod(extensionsType, "ToListAsync");
        }

        static MethodInfo FindQueryExtensionMethod(Type extensionsType, string name) {
            return extensionsType.GetMethods().First(m => {
                if(!m.IsGenericMethod || m.Name != name)
                    return false;

                var parameters = m.GetParameters();
                return parameters.Length == 2 && parameters[1].ParameterType == typeof(CancellationToken);
            });
        }

        protected Task<int> InvokeCountAsync(MethodInfo method, Expression expr) {
            var countArgument = ((MethodCallExpression)expr).Arguments[0];
            var query = Provider.CreateQuery(countArgument);
            return (Task<int>)InvokeQueryExtensionMethod(method, query.ElementType, query);
        }

        protected Task<IEnumerable<T>> InvokeToArrayAsync<T>(MethodInfo method, Expression expr) {
            var arrayTask = (Task<T[]>)InvokeQueryExtensionMethod(method, typeof(T), Provider.CreateQuery(expr));
            return arrayTask.ContinueWith(t => (IEnumerable<T>)t.Result);
        }

        protected Task<IEnumerable<T>> InvokeToListAsync<T>(MethodInfo method, Expression expr) {
            var listTask = (Task<List<T>>)InvokeQueryExtensionMethod(method, typeof(T), Provider.CreateQuery(expr));
            return listTask.ContinueWith(t => (IEnumerable<T>)t.Result);
        }

        object InvokeQueryExtensionMethod(MethodInfo method, Type elementType, IQueryable query) {
            return method
                .MakeGenericMethod(elementType)
                .Invoke(null, new object[] { query, CancellationToken });
        }
    }

}
