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

            var queryableSource = source.AsQueryable();

            if(options.IsCountQuery)
                return builder.BuildCountExpr().Compile()(queryableSource);

            var result = new DataSourceLoadResult();
            var query = builder.BuildLoadExpr().Compile()(queryableSource);

            if(options.RequireTotalCount)
                result.totalCount = builder.BuildCountExpr().Compile()(queryableSource);

            if(builder.HasGroups) {
                IEnumerable<Group> groups = new GroupHelper<T>(query.ToArray()).Group(builder.Group);

                if(builder.HasSummary)
                    result.summary = new AggregateCalculator<T>(groups, new Accessor<T>(), builder.TotalSummary, builder.GroupSummary).Run();

                groups = Paginate(groups, builder.Skip, builder.Take);
                CollapseGroups(groups, builder.Group);

                result.data = groups;
            } else if(builder.HasSummary) {
                var cached = query.ToArray();
                result.summary = new AggregateCalculator<T>(cached.Cast<object>(), new Accessor<T>(), builder.TotalSummary, null).Run();
                result.data = Paginate(cached, builder.Skip, builder.Take);
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
