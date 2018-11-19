﻿using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Data.RemoteGrouping;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data {

    class DataSourceLoaderImpl<S> {
        readonly IQueryable<S> Source;
        readonly DataSourceLoadOptionsBase Options;

        readonly QueryProviderInfo QueryProviderInfo;
        readonly DataSourceExpressionBuilder<S> Builder;
        readonly bool ShouldEmptyGroups;
        readonly bool CanUseRemoteGrouping;
        readonly bool SummaryIsTotalCountOnly;

        public DataSourceLoaderImpl(IQueryable<S> source, DataSourceLoadOptionsBase options) {
            QueryProviderInfo = new QueryProviderInfo(source.Provider);
            Builder = new DataSourceExpressionBuilder<S>(
                options,
                QueryProviderInfo.IsLinqToObjects,
                options.StringToLower.GetValueOrDefault(QueryProviderInfo.IsLinqToObjects),
                new AnonTypeNewTweaks {
                    AllowEmpty = !QueryProviderInfo.IsL2S,
                    AllowUnusedMembers = !QueryProviderInfo.IsL2S
                }
            );
            ShouldEmptyGroups = options.HasGroups && !options.Group.Last().GetIsExpanded();
            CanUseRemoteGrouping = options.RemoteGrouping ?? ShouldUseRemoteGrouping(QueryProviderInfo, options);
            SummaryIsTotalCountOnly = !options.HasGroupSummary && options.HasSummary && options.TotalSummary.All(i => i.SummaryType == AggregateName.COUNT);

            Source = source;
            Options = options;
        }

        static bool ShouldUseRemoteGrouping(QueryProviderInfo provider, DataSourceLoadOptionsBase options) {
            if(provider.IsLinqToObjects)
                return false;

            if(provider.IsEFCore) {
                // https://github.com/aspnet/EntityFrameworkCore/issues/2341
                // https://github.com/aspnet/EntityFrameworkCore/issues/11993
                // https://github.com/aspnet/EntityFrameworkCore/issues/11999
                if(provider.Version < new Version(2, 2, 0))
                    return false;

                bool HasAvg(SummaryInfo[] summary) {
                    return summary != null && summary.Any(i => i.SummaryType == "avg");
                }

                #warning Remove with https://github.com/aspnet/EntityFrameworkCore/issues/11711 fix
                if(HasAvg(options.TotalSummary) || HasAvg(options.GroupSummary))
                    return false;
            }

            return true;
        }

        public LoadResult Load() {
            if(Options.IsCountQuery)
                return new LoadResult { totalCount = ExecCount() };

            var result = new LoadResult();

            if(CanUseRemoteGrouping && ShouldEmptyGroups) {
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

                if(!Options.HasPrimaryKey && !Options.HasDefaultSort && (Options.Skip > 0 || Options.Take > 0)) {
                    if(QueryProviderInfo.IsEFClassic || QueryProviderInfo.IsEFCore)
                        Options.DefaultSort = EFSorting.FindSortableMember(typeof(S));
                    else if(QueryProviderInfo.IsXPO)
                        Options.DefaultSort = "this";
                }

                var deferPaging = Options.HasGroups || !CanUseRemoteGrouping && !SummaryIsTotalCountOnly && Options.HasSummary;
                var loadExpr = Builder.BuildLoadExpr(Source.Expression, !deferPaging);

                if(Options.HasAnySelect) {
                    ContinueWithGrouping(
                        ExecWithSelect(loadExpr),
                        result
                    );
                } else {
                    ContinueWithGrouping(
                        ExecExpr<S>(Source, loadExpr),
                        result
                    );
                }

                if(deferPaging)
                    result.data = Paginate(result.data, Options.Skip, Options.Take);

                if(ShouldEmptyGroups)
                    EmptyGroups(result.data, Options.Group.Length);
            }

            return result;
        }

        IEnumerable<IDictionary<string, object>> ExecWithSelect(Expression loadExpr) {
            var select = Options.GetFullSelect();

            if(Options.UseRemoteSelect)
                return SelectHelper.ConvertRemoteResult(ExecExpr<AnonType>(Source, loadExpr), select);

            return SelectHelper.Evaluate(ExecExpr<S>(Source, loadExpr), select);
        }

        void ContinueWithGrouping<R>(IEnumerable<R> loadResult, LoadResult result) {
            var accessor = new DefaultAccessor<R>();
            if(Options.HasGroups) {
                var groups = new GroupHelper<R>(accessor).Group(loadResult, Options.Group);
                if(Options.RequireGroupCount)
                    result.groupCount = groups.Count;
                ContinueWithAggregation(groups, accessor, result);
            } else {
                ContinueWithAggregation(loadResult, accessor, result);
            }
        }

        void ContinueWithAggregation<R>(IEnumerable data, IAccessor<R> accessor, LoadResult result) {
            if(CanUseRemoteGrouping && !SummaryIsTotalCountOnly && Options.HasSummary && !Options.HasGroups) {
                var groupingResult = ExecRemoteGrouping();
                result.totalCount = groupingResult.TotalCount;
                result.summary = groupingResult.Totals;
            } else {
                var totalCount = -1;

                if(Options.RequireTotalCount || SummaryIsTotalCountOnly)
                    totalCount = ExecCount();

                if(Options.RequireTotalCount)
                    result.totalCount = totalCount;

                if(SummaryIsTotalCountOnly) {
                    result.summary = Enumerable.Repeat((object)totalCount, Options.TotalSummary.Length).ToArray();
                } else if(Options.HasSummary) {
                    data = Buffer<R>(data);
                    result.summary = new AggregateCalculator<R>(data, accessor, Options.TotalSummary, Options.GroupSummary).Run();
                }
            }

            result.data = data;
        }

        int ExecCount() {
            var expr = Builder.BuildCountExpr(Source.Expression);
#if DEBUG
            Options.ExpressionWatcher?.Invoke(expr);
#endif
            return Source.Provider.Execute<int>(expr);
        }

        RemoteGroupingResult ExecRemoteGrouping() {
            return RemoteGroupTransformer.Run(
                typeof(S),
                ExecExpr<AnonType>(Source, Builder.BuildLoadGroupsExpr(Source.Expression)),
                Options.HasGroups ? Options.Group.Length : 0,
                Options.TotalSummary,
                Options.GroupSummary
            );
        }

        IEnumerable<R> ExecExpr<R>(IQueryable<S> source, Expression expr) {
            IEnumerable<R> result = source.Provider.CreateQuery<R>(expr);

#if DEBUG
            if(Options.UseEnumerableOnce)
                result = new EnumerableOnce<R>(result);

            Options.ExpressionWatcher?.Invoke(expr);
#endif

            return result;
        }

        static IEnumerable<T> ForceExecution<T>(IEnumerable<T> sequence) {
            foreach(var item in sequence)
                yield return item;
        }

        static IEnumerable Buffer<T>(IEnumerable data) {
            if(data is ICollection)
                return data;

            return Enumerable.ToArray((IEnumerable<T>)data);
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

                    if(g.items[0] is AnonType remoteGroup) {
                        g.count = (int)remoteGroup[0];
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
