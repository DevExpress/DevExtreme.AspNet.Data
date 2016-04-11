using DevExtreme.AspNet.Data.Aggregation;
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
                Group = options.Group,
                GroupSummary = options.GroupSummary,
                TotalSummary = options.TotalSummary
            };

            var q = source.AsQueryable();

            if(options.IsCountQuery)
                return builder.BuildCountExpr().Compile()(q);

            var result = new DataSourceLoadResult();
            var queryResult = builder.BuildLoadExpr().Compile()(q).ToArray();

            if(options.RequireTotalCount)
                result.totalCount = builder.BuildCountExpr().Compile()(q);

            if(builder.HasGroups) {
                IEnumerable<DevExtremeGroup> groups = new GroupHelper<T>(queryResult).Group(builder.Group);

                if(builder.HasSummary)
                    result.summary = new AggregateCalculator<T>(groups, new Accessor<T>(), builder.TotalSummary, builder.GroupSummary).Run();

                if(builder.Skip > 0)
                    groups = groups.Skip(builder.Skip);

                if(builder.Take > 0)
                    groups = groups.Take(builder.Take);

                result.data = groups;
            } else {
                if(builder.HasSummary)
                    result.summary = new AggregateCalculator<T>(queryResult.Cast<object>(), new Accessor<T>(), builder.TotalSummary, null).Run();

                result.data = queryResult;
            }

            if(result.IsDataOnly())
                return result.data;

            return result;
        }
    }

}
