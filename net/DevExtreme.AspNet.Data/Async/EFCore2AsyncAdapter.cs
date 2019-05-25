using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Async {

    class EFCore2AsyncAdapter : AsyncAdapter {
        static readonly MethodInfo CountAsyncMethod;
        static readonly MethodInfo ToArrayAsyncMethod;

        static EFCore2AsyncAdapter() {
            var extensionsType = Type.GetType("Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions, Microsoft.EntityFrameworkCore");

            // https://docs.microsoft.com/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.countasync
            CountAsyncMethod = FindCountAsyncMethod(extensionsType);

            // https://docs.microsoft.com/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.toarrayasync
            ToArrayAsyncMethod = FindToArrayAsyncMethod(extensionsType);
        }

        public EFCore2AsyncAdapter(IQueryProvider provider, CancellationToken cancellationToken)
            : base(provider, cancellationToken) {
        }

        public override Task<int> CountAsync(Expression expr) {
            return InvokeCountAsync(CountAsyncMethod, expr);
        }

        public override Task<IEnumerable<T>> ToEnumerableAsync<T>(Expression expr) {
            return InvokeToArrayAsync<T>(ToArrayAsyncMethod, expr);
        }
    }
}
