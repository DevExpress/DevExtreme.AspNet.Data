using DevExtreme.AspNet.Data.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExtreme.AspNet.Data {

    public class DataSourceLoader {

        public static object Load<T>(IEnumerable<T> source, DataSourceLoadOptionsBase options) {
            var queryableSource = source.AsQueryable();
            var builder = new DataSourceExpressionBuilder<T>(options, queryableSource is EnumerableQuery);

            if(options.IsCountQuery)
                return builder.BuildCountExpr().Compile()(queryableSource);

            var accessor = new Accessor<T>();
            var result = new DataSourceLoadResult();
            var query = builder.BuildLoadExpr().Compile()(queryableSource);

            if(options.RequireTotalCount)
                result.totalCount = builder.BuildCountExpr().Compile()(queryableSource);

            if(options.HasGroups) {
                IEnumerable<Group> groups = new GroupHelper<T>(query.ToArray(), accessor).Group(options.Group);

                if(options.HasSummary)
                    result.summary = new AggregateCalculator<T>(groups, accessor, options.TotalSummary, options.GroupSummary).Run();

                groups = Paginate(groups, options.Skip, options.Take);
                CollapseGroups(groups, options.Group);

                result.data = groups;
            } else if(options.HasSummary) {
                var cached = query.ToArray();
                result.summary = new AggregateCalculator<T>(cached.Cast<object>(), accessor, options.TotalSummary, null).Run();
                result.data = Paginate(cached, options.Skip, options.Take);
            } else {
                result.data = query;
            }

            if(result.IsDataOnly())
                return result.data;

            return result;
        }

        static IEnumerable<T> Paginate<T>(IEnumerable<T> data, int skip, int take) {
            if(skip > 0)
                data = data.Skip(skip);

            if(take > 0)
                data = data.Take(take);

            return data;
        }

        static IEnumerable<Group> CollapseGroups(IEnumerable<Group> groups, IEnumerable<GroupingInfo> grouping) {
#warning can collapse non-leaf groups?
            var isLeafGroup = grouping.Count() < 2;
            var thisGrouping = grouping.First();
            var isExpanded = !thisGrouping.IsExpanded.HasValue || thisGrouping.IsExpanded.Value;

            foreach(var g in groups) {
                if(isLeafGroup) {
                    if(!isExpanded) {
                        g.count = g.items.Count;
                        g.items = null;
                    }
                } else {
                    CollapseGroups(g.items.Cast<Group>(), grouping.Skip(1));
                }
            }

            return groups;
        }
    }

}
