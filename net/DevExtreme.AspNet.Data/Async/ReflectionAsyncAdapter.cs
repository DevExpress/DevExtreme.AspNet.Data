using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Async {

    class ReflectionAsyncAdapter : IAsyncAdapter {
        QueryProviderInfo _providerInfo;

        public ReflectionAsyncAdapter(QueryProviderInfo providerInfo) {
            _providerInfo = providerInfo;
        }

        bool IsEF6 => _providerInfo.IsEFClassic && _providerInfo.Version.Major >= 6;
        bool IsEFCore => _providerInfo.IsEFCore;
        bool IsNH => _providerInfo.IsNH;

        public Task<int> CountAsync(IQueryProvider provider, Expression expr, CancellationToken cancellationToken) {
            if(IsEFCore)
                return InvokeCountAsync(EFCore2Methods.CountAsyncMethod, provider, expr, cancellationToken);

            if(IsEF6)
                return InvokeCountAsync(EF6Methods.CountAsyncMethod, provider, expr, cancellationToken);

            if(IsNH)
                return InvokeCountAsync(NHMethods.CountAsyncMethod, provider, expr, cancellationToken);

            throw NotSupported();
        }

        public Task<IEnumerable<T>> ToEnumerableAsync<T>(IQueryProvider provider, Expression expr, CancellationToken cancellationToken) {
            if(IsEFCore)
                return InvokeToArrayAsync<T>(EFCore2Methods.ToArrayAsyncMethod, provider, expr, cancellationToken);

            if(IsEF6)
                return InvokeToArrayAsync<T>(EF6Methods.ToArrayAsyncMethod, provider, expr, cancellationToken);

            if(IsNH)
                return InvokeToListAsync<T>(NHMethods.ToListAsyncMethod, provider, expr, cancellationToken);


            throw NotSupported();
        }

        static Exception NotSupported() {
#warning TODO message
            throw new NotSupportedException();
        }

        static class EF6Methods {
            public static readonly MethodInfo CountAsyncMethod;
            public static readonly MethodInfo ToArrayAsyncMethod;
            static EF6Methods() {
                var extensionsType = Type.GetType("System.Data.Entity.QueryableExtensions, EntityFramework");
                CountAsyncMethod = FindCountAsyncMethod(extensionsType);
                ToArrayAsyncMethod = FindToArrayAsyncMethod(extensionsType);
            }
        }

        static class EFCore2Methods {
            public static readonly MethodInfo CountAsyncMethod;
            public static readonly MethodInfo ToArrayAsyncMethod;
            static EFCore2Methods() {
                var extensionsType = Type.GetType("Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions, Microsoft.EntityFrameworkCore");
                CountAsyncMethod = FindCountAsyncMethod(extensionsType);
                ToArrayAsyncMethod = FindToArrayAsyncMethod(extensionsType);
            }
        }

        static class NHMethods {
            public static readonly MethodInfo CountAsyncMethod;
            public static readonly MethodInfo ToListAsyncMethod;
            static NHMethods() {
                var extensionsType = Type.GetType("NHibernate.Linq.LinqExtensionMethods, NHibernate");
                CountAsyncMethod = FindCountAsyncMethod(extensionsType);
                ToListAsyncMethod = FindToListAsyncMethod(extensionsType);
            }
        }

        static MethodInfo FindCountAsyncMethod(Type extensionsType) {
            return FindQueryExtensionMethod(extensionsType, "CountAsync");
        }

        static MethodInfo FindToArrayAsyncMethod(Type extensionsType) {
            return FindQueryExtensionMethod(extensionsType, "ToArrayAsync");
        }

        static MethodInfo FindToListAsyncMethod(Type extensionsType) {
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

        static Task<int> InvokeCountAsync(MethodInfo method, IQueryProvider provider, Expression expr, CancellationToken cancellationToken) {
            var countArgument = ((MethodCallExpression)expr).Arguments[0];
            var query = provider.CreateQuery(countArgument);
            return (Task<int>)InvokeQueryExtensionMethod(method, query.ElementType, query, cancellationToken);
        }

        static Task<IEnumerable<T>> InvokeToArrayAsync<T>(MethodInfo method, IQueryProvider provider, Expression expr, CancellationToken cancellationToken) {
            var arrayTask = (Task<T[]>)InvokeQueryExtensionMethod(method, typeof(T), provider.CreateQuery(expr), cancellationToken);
            return arrayTask.ContinueWith(t => (IEnumerable<T>)t.Result);
        }

        static Task<IEnumerable<T>> InvokeToListAsync<T>(MethodInfo method, IQueryProvider provider, Expression expr, CancellationToken cancellationToken) {
            var listTask = (Task<List<T>>)InvokeQueryExtensionMethod(method, typeof(T), provider.CreateQuery(expr), cancellationToken);
            return listTask.ContinueWith(t => (IEnumerable<T>)t.Result);
        }

        static object InvokeQueryExtensionMethod(MethodInfo method, Type elementType, IQueryable query, CancellationToken cancellationToken) {
            return method
                .MakeGenericMethod(elementType)
                .Invoke(null, new object[] { query, cancellationToken });
        }

    }

}
