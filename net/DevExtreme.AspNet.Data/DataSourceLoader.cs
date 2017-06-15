using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Data.RemoteGrouping;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data {

    public class DataSourceLoader {

        public static object Load<T>(IEnumerable<T> source, DataSourceLoadOptionsBase options) {
            return Load(source.AsQueryable(), options);
        }

        public static object Load<T>(IQueryable<T> source, DataSourceLoadOptionsBase options) {
            return new Impl<T>(source, options).Run();
        }


        class Impl<S> {
            readonly IQueryable<S> Source;
            readonly DataSourceLoadOptionsBase Options;

            readonly DataSourceExpressionBuilder<S> Builder;
            readonly bool EmptyGroups;
            readonly bool CanUseRemoteGrouping;

            public Impl(IQueryable<S> source, DataSourceLoadOptionsBase options) {
                var isLinqToObjects = source is EnumerableQuery;

                Builder = new DataSourceExpressionBuilder<S>(options, isLinqToObjects);
                EmptyGroups = options.HasGroups && !options.Group.Last().GetIsExpanded();
                CanUseRemoteGrouping = options.RemoteGrouping.HasValue ? options.RemoteGrouping.Value : !isLinqToObjects;

                Source = source;
                Options = options;
            }

            public object Run() {
                if(Options.IsCountQuery)
                    return ExecCount();

                DataSourceLoadResult result;

                if(CanUseRemoteGrouping && EmptyGroups) {
                    result = LoadGroupsOnly();
                } else {
                    if(!Options.HasPrimaryKey)
                        Options.PrimaryKey = Utils.GetPrimaryKey(typeof(S));

                    if(!Options.HasPrimaryKey && (Options.Skip > 0 || Options.Take > 0) && Compat.IsEntityFramework(Source.Provider))
                        Options.DefaultSort = EFSorting.FindSortableMember(typeof(S));

                    if(Options.HasSelect)
                        result = LoadData<AnonType>();
                    else
                        result = LoadData<S>();

                    if(EmptyGroups)
                        EmptyGroups(result.data, Options.Group.Length);
                }

                if(result.IsDataOnly())
                    return result.data;

                return result;
            }

            DataSourceLoadResult LoadData<R>() {
                var deferPaging = Options.HasGroups || Options.HasSummary && !CanUseRemoteGrouping;
                var dataQuery = AppendExpr<S, R>(Source, Builder.BuildLoadExpr(Source.Expression, !deferPaging), Options);

                IEnumerable data = dataQuery;

                var result = new DataSourceLoadResult();
                var accessor = new DefaultAccessor<R>();

                if(Options.HasGroups) {
                    data = new GroupHelper<R>(accessor).Group(dataQuery, Options.Group);
                    if(Options.RequireGroupCount) {
                        result.groupCount = (data as IList).Count;
                    }
                }

                if(CanUseRemoteGrouping && Options.HasSummary && !Options.HasGroups) {
                    var groupingResult = ExecRemoteGrouping();
                    result.totalCount = groupingResult.TotalCount;
                    result.summary = groupingResult.Totals;
                } else {
                    if(Options.RequireTotalCount)
                        result.totalCount = ExecCount();

                    if(Options.HasSummary) {
                        data = Buffer<S>(data);
                        result.summary = new AggregateCalculator<R>(data, accessor, Options.TotalSummary, Options.GroupSummary).Run();
                    }
                }

                if(deferPaging)
                    data = Paginate(data, Options.Skip, Options.Take);

                result.data = data;
                return result;
            }

            DataSourceLoadResult LoadGroupsOnly() {
                var result = new DataSourceLoadResult();
                var groupingResult = ExecRemoteGrouping();

                EmptyGroups(groupingResult.Groups, Options.Group.Length);

                result.data = Paginate(groupingResult.Groups, Options.Skip, Options.Take);
                result.summary = groupingResult.Totals;
                result.totalCount = groupingResult.TotalCount;

                if(Options.RequireGroupCount)
                    result.groupCount = groupingResult.Groups.Count();

                return result;
            }

            int ExecCount() {
                return Source.Provider.Execute<int>(Builder.BuildCountExpr(Source.Expression));
            }

            RemoteGroupingResult ExecRemoteGrouping() {
                return RemoteGroupTransformer.Run(
                    AppendExpr<S, AnonType>(Source, Builder.BuildLoadGroupsExpr(Source.Expression), Options),
                    Options.HasGroups ? Options.Group.Length : 0,
                    Options.TotalSummary,
                    Options.GroupSummary
                );
            }
        }

        static IEnumerable Buffer<T>(IEnumerable data) {
            var q = data as IQueryable<T>;
            if(q != null)
                return q.ToArray();

            return data;
        }

        static IQueryable<R> AppendExpr<S, R>(IQueryable<S> source, Expression expr, DataSourceLoadOptionsBase options) {
            var result = source.Provider.CreateQuery<R>(expr);

#if DEBUG
            if(options.UseQueryableOnce)
                result = new QueryableOnce<R>(result);

            if(options.ExpressionWatcher != null)
                options.ExpressionWatcher(result.Expression);
#endif

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
                    var remoteGroup = g.items[0] as AnonType;

                    if(remoteGroup != null) {
                        g.count = (int)remoteGroup[RemoteGroupTypeMarkup.CountIndex];
                    } else {
                        g.count = g.items.Count;
                    }

                    g.items = null;
                } else {
                    EmptyGroups(g.items, level - 1);
                }
            }
        }
    }

}
