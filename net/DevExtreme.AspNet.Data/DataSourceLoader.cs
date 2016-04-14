using DevExtreme.AspNet.Data.Aggregation;
using System;
using System.Collections;
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
            var emptyGroups = options.HasGroups && !options.Group.Last().GetIsExpanded();

            if(options.RequireTotalCount)
                result.totalCount = builder.BuildCountExpr().Compile()(queryableSource);
          
            var deferPaging = options.HasGroups || options.HasSummary;
            var q = builder.BuildLoadExpr().Compile();

            IEnumerable data = null;

            if(options.HasGroups)
                data = new GroupHelper<T>(accessor).Group(q(queryableSource), options.Group);
            else
                data = q(queryableSource).ToArray();

            // at this point, query is executed and data is in memory

            if(options.HasSummary)
                result.summary = new AggregateCalculator<T>(data, accessor, options.TotalSummary, options.GroupSummary).Run();            

            if(deferPaging)
                data = Paginate(data, options.Skip, options.Take);

            if(emptyGroups && options.HasGroups)
                EmptyGroups(data, options.Group.Length);

            result.data = data;

            if(result.IsDataOnly())
                return result.data;

            return result;
        }

        static IEnumerable Paginate(IEnumerable data, int skip, int take) {
            if(skip < 1 && take < 1)
                return data;

            var typed = data.Cast<object>();

            if(skip > 0)
                typed = typed.Skip(skip);

            if(take > 0)
                typed = typed.Take(take);

            return typed;
        }

        static void EmptyGroups(IEnumerable groups, int level) {
            foreach(Group g in groups) {
                if(level < 2) {
                    g.count = g.items.Count;
                    g.items = null;
                } else {
                    EmptyGroups(g.items, level - 1);
                }
            }
        }
    }

}
