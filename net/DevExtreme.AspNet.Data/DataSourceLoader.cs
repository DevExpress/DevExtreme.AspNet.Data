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

                var result = new DataSourceLoadResult();

                if(CanUseRemoteGrouping && EmptyGroups) {
                    var groupingResult = ExecRemoteGrouping();

                    EmptyGroups(groupingResult.Groups, Options.Group.Length);

                    result.data = Paginate(groupingResult.Groups, Options.Skip, Options.Take);
                    result.summary = groupingResult.Totals;
                    result.totalCount = groupingResult.TotalCount;

                    if(Options.RequireGroupCount)
                        result.groupCount = groupingResult.Groups.Count();
                } else {
                    if(!Options.HasPrimaryKey)
                        Options.PrimaryKey = Utils.GetPrimaryKey(typeof(S));

                    if(!Options.HasPrimaryKey && (Options.Skip > 0 || Options.Take > 0) && Compat.IsEntityFramework(Source.Provider))
                        Options.DefaultSort = EFSorting.FindSortableMember(typeof(S));

                    var deferPaging = Options.HasGroups || Options.HasSummary && !CanUseRemoteGrouping;
                    var loadExpr = Builder.BuildLoadExpr(Source.Expression, !deferPaging);

                    if(Options.HasSelect) {
                        ContinueWithGrouping(
                            AppendExpr<S, AnonType>(Source, loadExpr, Options)
                                .AsEnumerable()
                                .Select(i => AnonToDict(i, Options.Select)),
#warning TODO
                            null,
                            result
                        );
                    } else {
                        ContinueWithGrouping(
                            AppendExpr<S, S>(Source, loadExpr, Options),
                            new DefaultAccessor<S>(),
                            result
                        );
                    }

                    if(deferPaging)
                        result.data = Paginate(result.data, Options.Skip, Options.Take);

                    if(EmptyGroups)
                        EmptyGroups(result.data, Options.Group.Length);
                }

                if(result.IsDataOnly())
                    return result.data;

                return result;
            }

            void ContinueWithGrouping<R>(IEnumerable<R> query, IAccessor<R> accessor, DataSourceLoadResult result) {
                IEnumerable data = query;

                if(Options.HasGroups) {
                    data = new GroupHelper<R>(accessor).Group(query, Options.Group);
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
                        data = Buffer<R>(data);
                        result.summary = new AggregateCalculator<R>(data, accessor, Options.TotalSummary, Options.GroupSummary).Run();
                    }
                }

                result.data = data;
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

        static Dictionary<string, object> AnonToDict(AnonType obj, string[] names) {
            var dict = new Dictionary<string, object>();
            for(var i = 0; i < names.Length; i++)
                dict[names[i]] = obj[i];
            return dict;
        }
    }

}
