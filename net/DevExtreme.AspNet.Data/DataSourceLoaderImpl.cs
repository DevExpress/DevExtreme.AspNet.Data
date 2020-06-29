using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Async;
using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Data.RemoteGrouping;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    class DataSourceLoaderImpl<S> {
        readonly IQueryable Source;
        readonly DataSourceLoadContext Context;
        readonly Func<Expression, ExpressionExecutor> CreateExecutor;

#if DEBUG
        readonly Action<Expression> ExpressionWatcher;
        readonly bool UseEnumerableOnce;
#endif

        public DataSourceLoaderImpl(IQueryable source, DataSourceLoadOptionsBase options, CancellationToken cancellationToken, bool sync) {
            var providerInfo = new QueryProviderInfo(source.Provider);

            Source = source;
            Context = new DataSourceLoadContext(options, providerInfo, Source.ElementType);
            CreateExecutor = expr => new ExpressionExecutor(Source.Provider, expr, providerInfo, cancellationToken, sync, options.AllowAsyncOverSync);

#if DEBUG
            ExpressionWatcher = options.ExpressionWatcher;
            UseEnumerableOnce = options.UseEnumerableOnce;
#endif
        }

        DataSourceExpressionBuilder CreateBuilder() => new DataSourceExpressionBuilder(Source.Expression, Context);

        public async Task<LoadResult> LoadAsync() {
            if(Context.IsCountQuery)
                return new LoadResult { totalCount = await ExecTotalCountAsync() };

            var result = new LoadResult();

            if(Context.UseRemoteGrouping && Context.ShouldEmptyGroups) {
                var remotePaging = Context.HasPaging && Context.Group.Count == 1;
                var groupingResult = await ExecRemoteGroupingAsync(remotePaging, false, remotePaging);

                EmptyGroups(groupingResult.Groups, Context.Group.Count);

                result.data = groupingResult.Groups;
                if(!remotePaging)
                    result.data = Paginate(result.data, Context.Skip, Context.Take);

                if(remotePaging) {
                    if(Context.HasTotalSummary) {
                        var totalsResult = await ExecRemoteTotalsAsync();
                        result.summary = totalsResult.Totals;
                        result.totalCount = totalsResult.TotalCount;
                    } else if(Context.RequireTotalCount) {
                        result.totalCount = await ExecTotalCountAsync();
                    }
                } else {
                    result.summary = groupingResult.Totals;
                    result.totalCount = groupingResult.TotalCount;
                }

                if(Context.RequireGroupCount) {
                    result.groupCount = remotePaging
                        ? await ExecCountAsync(CreateBuilder().BuildGroupCountExpr())
                        : groupingResult.Groups.Count();
                }
            } else {
                var deferPaging = Context.HasGroups || !Context.UseRemoteGrouping && !Context.SummaryIsTotalCountOnly && Context.HasSummary;

                Expression loadExpr;

                if(!deferPaging && Context.PaginateViaPrimaryKey && Context.Take > 0) {
                    if(!Context.HasPrimaryKey) {
                        throw new InvalidOperationException(nameof(DataSourceLoadOptionsBase.PaginateViaPrimaryKey)
                            + " requires a primary key."
                            + " Specify it via the " + nameof(DataSourceLoadOptionsBase.PrimaryKey) + " property.");
                    }

                    var loadKeysExpr = CreateBuilder().BuildLoadExpr(true, selectOverride: Context.PrimaryKey);
                    var keyTuples = await ExecExprAsync<AnonType>(loadKeysExpr);

                    loadExpr = CreateBuilder().BuildLoadExpr(false, filterOverride: FilterFromKeys(keyTuples));
                } else {
                    loadExpr = CreateBuilder().BuildLoadExpr(!deferPaging);
                }

                if(Context.HasAnySelect) {
                    await ContinueWithGroupingAsync(
                        await ExecWithSelectAsync(loadExpr),
                        result
                    );
                } else {
                    await ContinueWithGroupingAsync(
                        await ExecExprAsync<S>(loadExpr),
                        result
                    );
                }

                if(deferPaging)
                    result.data = Paginate(result.data, Context.Skip, Context.Take);

                if(Context.ShouldEmptyGroups)
                    EmptyGroups(result.data, Context.Group.Count);
            }

            return result;
        }

        async Task<IEnumerable<ExpandoObject>> ExecWithSelectAsync(Expression loadExpr) {
            if(Context.UseRemoteSelect)
                return SelectHelper.ConvertRemoteResult(await ExecExprAsync<AnonType>(loadExpr), Context.FullSelect);

            return SelectHelper.Evaluate(await ExecExprAsync<S>(loadExpr), Context.FullSelect);
        }

        async Task ContinueWithGroupingAsync<R>(IEnumerable<R> loadResult, LoadResult result) {
            var accessor = new DefaultAccessor<R>();
            if(Context.HasGroups) {
                var groups = new GroupHelper<R>(accessor).Group(loadResult, Context.Group);
                if(Context.RequireGroupCount)
                    result.groupCount = groups.Count;
                await ContinueWithAggregationAsync(groups, accessor, result);
            } else {
                await ContinueWithAggregationAsync(loadResult, accessor, result);
            }
        }

        async Task ContinueWithAggregationAsync<R>(IEnumerable data, IAccessor<R> accessor, LoadResult result) {
            if(Context.UseRemoteGrouping && !Context.SummaryIsTotalCountOnly && Context.HasSummary && !Context.HasGroups) {
                var totalsResult = await ExecRemoteTotalsAsync();
                result.totalCount = totalsResult.TotalCount;
                result.summary = totalsResult.Totals;
            } else {
                var totalCount = -1;

                if(Context.RequireTotalCount || Context.SummaryIsTotalCountOnly)
                    totalCount = await ExecTotalCountAsync();

                if(Context.RequireTotalCount)
                    result.totalCount = totalCount;

                if(Context.SummaryIsTotalCountOnly) {
                    result.summary = Enumerable.Repeat((object)totalCount, Context.TotalSummary.Count).ToArray();
                } else if(Context.HasSummary) {
                    data = Buffer<R>(data);
                    result.summary = new AggregateCalculator<R>(data, accessor, Context.TotalSummary, Context.GroupSummary).Run();
                }
            }

            result.data = data;
        }

        Task<int> ExecCountAsync(Expression expr) {
#if DEBUG
            ExpressionWatcher?.Invoke(expr);
#endif

            var executor = CreateExecutor(expr);

            if(Context.RequireQueryableChainBreak)
                executor.BreakQueryableChain();

            return executor.CountAsync();
        }

        Task<int> ExecTotalCountAsync() => ExecCountAsync(CreateBuilder().BuildCountExpr());

        Task<RemoteGroupingResult> ExecRemoteTotalsAsync() => ExecRemoteGroupingAsync(false, true, false);

        async Task<RemoteGroupingResult> ExecRemoteGroupingAsync(bool remotePaging, bool suppressGroups, bool suppressTotals) {
            return RemoteGroupTransformer.Run(
                Source.ElementType,
                await ExecExprAsync<AnonType>(CreateBuilder().BuildLoadGroupsExpr(remotePaging, suppressGroups, suppressTotals)),
                !suppressGroups && Context.HasGroups ? Context.Group.Count : 0,
                !suppressTotals ? Context.TotalSummary : null,
                !suppressGroups ? Context.GroupSummary : null
            );
        }

        async Task<IEnumerable<R>> ExecExprAsync<R>(Expression expr) {
#if DEBUG
            ExpressionWatcher?.Invoke(expr);
#endif

            var executor = CreateExecutor(expr);

            if(Context.RequireQueryableChainBreak)
                executor.BreakQueryableChain();

            var result = await executor.ToEnumerableAsync<R>();

#if DEBUG
            if(UseEnumerableOnce)
                result = new EnumerableOnce<R>(result);
#endif

            return result;
        }

        IList FilterFromKeys(IEnumerable<AnonType> keyTuples) {
            var result = new List<object>();
            var key = Context.PrimaryKey;
            var keyLength = key.Count;

            foreach(var tuple in keyTuples) {
                if(result.Count > 0)
                    result.Add("or");

                void AddCondition(IList container, int index) {
                    container.Add(new object[] { key[index], tuple[index] });
                }

                if(keyLength == 1) {
                    AddCondition(result, 0);
                } else {
                    var group = new List<object>();
                    for(var i = 0; i < keyLength; i++)
                        AddCondition(group, i);
                    result.Add(group);
                }
            }

            return result;
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
