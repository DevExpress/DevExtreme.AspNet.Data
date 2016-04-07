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
                Sort = options.Sort,
                Group = options.Group
            };

            var q = source.AsQueryable();

            if(options.IsCountQuery)
                return builder.BuildCountExpr().Compile()(q);

            var loadResult = builder.BuildLoadExpr().Compile()(q);

            object data = null;
            if(builder.HasGroups)
                data = new GroupHelper<T>(loadResult).Group(builder.GetGroupSelectors());
            else
                data = loadResult;

            if(options.RequireTotalCount)
                return new Dictionary<string, object> {
                    { "data", data },
                    { "totalCount", builder.BuildCountExpr().Compile()(q) }
                };

            return data;
        }

    }

}
