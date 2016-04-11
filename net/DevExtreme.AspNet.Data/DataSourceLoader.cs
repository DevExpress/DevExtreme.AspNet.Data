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

            object data = LoadData(builder, q);

            if(options.RequireTotalCount)
                return new Dictionary<string, object> {
                    { "data", data },
                    { "totalCount", builder.BuildCountExpr().Compile()(q) }
                };

            return data;
        }

        static object LoadData<T>(DataSourceExpressionBuilder<T> builder, IQueryable<T> q) {
            var loadResult = builder.BuildLoadExpr().Compile()(q);
            if(!builder.HasGroups)
                return loadResult;

            IEnumerable<DevExtremeGroup> groups = new GroupHelper<T>(loadResult).Group(builder.Group);

            if(builder.Skip > 0)
                groups = groups.Skip(builder.Skip);

            if(builder.Take > 0)
                groups = groups.Take(builder.Take);

            return groups;
        }

    }

}
