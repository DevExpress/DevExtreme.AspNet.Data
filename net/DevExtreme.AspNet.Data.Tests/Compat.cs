using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DevExtreme.AspNet.Data.Tests {

    static class Compat {

        public static string ExpectedConvert(object subj, string type) {
            // Difference introduced in https://github.com/dotnet/corefx/pull/11482

            var text = new StringBuilder("Convert(").Append(subj);
#if !NET4
            text.Append(", ").Append(type);
#endif
            return text.Append(")").ToString();
        }

        public static DataSourceExpressionBuilder CreateDataSourceExpressionBuilder<T>(DataSourceLoadOptionsBase options) {
            var source = new EnumerableQuery<T>(Expression.Parameter(typeof(IQueryable<T>), "data"));
            return CreateDataSourceExpressionBuilder(source, options);
        }

        public static DataSourceExpressionBuilder CreateDataSourceExpressionBuilder<T>(IQueryable<T> source, DataSourceLoadOptionsBase options) {
            return new DataSourceExpressionBuilder(
                source.Expression,
                new DataSourceLoadContext(
                    options,
                    new QueryProviderInfo(source.Provider),
                    typeof(T)
                )
            );
        }
    }

}
