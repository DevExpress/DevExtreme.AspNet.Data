using System;
using System.Linq;
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

        public static OLD_DataSourceExpressionBuilder<T> CreateDataSourceExpressionBuilder<T>(DataSourceLoadOptionsBase options, bool guardNulls = false) {
            return new OLD_DataSourceExpressionBuilder<T>(
                new DataSourceLoadContext(
                    options,
                    new QueryProviderInfo(new T[0].AsQueryable().Provider),
                    typeof(T)
                ),
                guardNulls
            );
        }
    }

}
