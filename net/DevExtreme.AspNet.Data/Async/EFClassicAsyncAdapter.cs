using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Async {

    class EFClassicAsyncAdapter : AsyncAdapter {
        static readonly MethodInfo CountAsyncMethod;
        static readonly MethodInfo ToArrayAsyncMethod;

        static EFClassicAsyncAdapter() {
            var extensionsType = Type.GetType("System.Data.Entity.QueryableExtensions, EntityFramework");

            // https://docs.microsoft.com/dotnet/api/system.data.entity.queryableextensions.countasync
            CountAsyncMethod = FindCountAsyncMethod(extensionsType);

            // https://docs.microsoft.com/dotnet/api/system.data.entity.queryableextensions.toarrayasync
            ToArrayAsyncMethod = FindToArrayAsyncMethod(extensionsType);
        }

        public EFClassicAsyncAdapter(IQueryProvider provider, CancellationToken cancellationToken)
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
