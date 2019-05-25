using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Async {

    class NHAsyncAdapter : AsyncAdapter {
        static readonly MethodInfo CountAsyncMethod;
        static readonly MethodInfo ToListAsyncMethod;

        static NHAsyncAdapter() {
            var extensionsType = Type.GetType("NHibernate.Linq.LinqExtensionMethods, NHibernate");

            // https://github.com/nhibernate/nhibernate-core/blob/5.2.5/src/NHibernate/Linq/LinqExtensionMethods.cs#L150
            CountAsyncMethod = FindCountAsyncMethod(extensionsType);

            // https://github.com/nhibernate/nhibernate-core/blob/5.2.5/src/NHibernate/Linq/LinqExtensionMethods.cs#L2374
            ToListAsyncMethod = FindToListAsyncMethod(extensionsType);
        }

        public NHAsyncAdapter(IQueryProvider provider, CancellationToken cancellationToken)
            : base(provider, cancellationToken) {
        }

        public override Task<int> CountAsync(Expression expr) {
            return InvokeCountAsync(CountAsyncMethod, expr);
        }

        public override Task<IEnumerable<T>> ToEnumerableAsync<T>(Expression expr) {
            return InvokeToListAsync<T>(ToListAsyncMethod, expr);
        }
    }

}
