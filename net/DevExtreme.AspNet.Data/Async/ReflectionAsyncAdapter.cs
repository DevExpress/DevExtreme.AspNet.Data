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
        bool IsXPO => _providerInfo.IsXPO;

        public bool IsSupportedProvider => IsEFCore || IsEF6 || IsNH || IsXPO;

        public Task<int> CountAsync(IQueryProvider provider, Expression expr, CancellationToken cancellationToken) {
            MethodInfo GetCountAsyncMethod() {
                if(IsEFCore)
                    return EFCoreMethods.CountAsyncMethod;

                if(IsEF6)
                    return EF6Methods.CountAsyncMethod;

                if(IsNH)
                    return NHMethods.CountAsyncMethod;

                if(IsXPO)
                    return XpoMethods.CountAsyncMethod;

                throw new NotSupportedException();
            }

            return InvokeCountAsync(GetCountAsyncMethod(), provider, expr, cancellationToken);
        }

        public Task<IEnumerable<T>> ToEnumerableAsync<T>(IQueryProvider provider, Expression expr, CancellationToken cancellationToken) {
            if(IsEFCore)
                return InvokeToListAsync<T>(EFCoreMethods.ToListAsyncMethod, provider, expr, cancellationToken);

            if(IsEF6)
                return InvokeToListAsync<T>(EF6Methods.ToListAsyncMethod, provider, expr, cancellationToken);

            if(IsNH)
                return InvokeToListAsync<T>(NHMethods.ToListAsyncMethod, provider, expr, cancellationToken);

            if(IsXPO)
                return InvokeToArrayAsync<T>(XpoMethods.ToArrayAsyncMethod, provider, expr, cancellationToken);

            throw new NotSupportedException();
        }

        static class EF6Methods {
            public static readonly MethodInfo CountAsyncMethod;
            public static readonly MethodInfo ToListAsyncMethod;
            static EF6Methods() {
                var extensionsType = Type.GetType("System.Data.Entity.QueryableExtensions, EntityFramework");
                CountAsyncMethod = FindCountAsyncMethod(extensionsType);
                ToListAsyncMethod = FindToListAsyncMethod(extensionsType);
            }
        }

        static class EFCoreMethods {
            public static readonly MethodInfo CountAsyncMethod;
            public static readonly MethodInfo ToListAsyncMethod;
            static EFCoreMethods() {
                var extensionsType = Type.GetType("Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions, Microsoft.EntityFrameworkCore");
                CountAsyncMethod = FindCountAsyncMethod(extensionsType);
                ToListAsyncMethod = FindToListAsyncMethod(extensionsType);
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

        static class XpoMethods {
            public static readonly MethodInfo CountAsyncMethod;
            public static readonly MethodInfo ToArrayAsyncMethod;
            static XpoMethods() {
                var asm = Array.Find(AppDomain.CurrentDomain.GetAssemblies(), a => a.FullName.StartsWith("DevExpress.Xpo.v"));
                var extensionsType = asm.GetType("DevExpress.Xpo.XPQueryExtensions");
                CountAsyncMethod = FindCountAsyncMethod(extensionsType);
                ToArrayAsyncMethod = FindToArrayAsyncMethod(extensionsType);
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

        async static Task<IEnumerable<T>> InvokeToArrayAsync<T>(MethodInfo method, IQueryProvider provider, Expression expr, CancellationToken cancellationToken) {
            return await (Task<T[]>)InvokeQueryExtensionMethod(method, typeof(T), provider.CreateQuery(expr), cancellationToken);
        }

        async static Task<IEnumerable<T>> InvokeToListAsync<T>(MethodInfo method, IQueryProvider provider, Expression expr, CancellationToken cancellationToken) {
            return await (Task<List<T>>)InvokeQueryExtensionMethod(method, typeof(T), provider.CreateQuery(expr), cancellationToken);
        }

        static object InvokeQueryExtensionMethod(MethodInfo method, Type elementType, IQueryable query, CancellationToken cancellationToken) {
            return method
                .MakeGenericMethod(elementType)
                .Invoke(null, new object[] { query, cancellationToken });
        }

    }

}
