using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExtreme.AspNet.Data {

    public class DataSourceLoader {

        public static object Load<T>(IEnumerable<T> source, DataSourceLoadOptionsBase options) {
            var builder = new DataSourceExpressionBuilder<T> {
                Skip = options.Skip,
                Take = options.Take,
                Filter = options.Filter,
                Sort = options.Sort
            };

            var q = source.AsQueryable();

            if(options.IsCountQuery)
                return builder.Build(true).Compile().DynamicInvoke(q);

            var data = builder.Build(false).Compile().DynamicInvoke(q);

            if(options.RequireTotalCount)
                return new Dictionary<string, object> {
                    { "data", data  },
                    { "totalCount",  builder.Build(true).Compile().DynamicInvoke(q) }
                };

            return data;
        }

    }

}
