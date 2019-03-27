using DevExtreme.AspNet.Data.Aggregation;
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

namespace DevExtreme.AspNet.Data {

    class DataSourceLoaderImpl<S> {
        readonly IQueryable<S> Source;
        readonly DataSourceLoadContext Context;
        readonly DataSourceExpressionBuilder<S> Builder;

#if DEBUG
        readonly Action<Expression> ExpressionWatcher;
        readonly bool UseEnumerableOnce;
#endif

        public DataSourceLoaderImpl(IQueryable<S> source, DataSourceLoadOptionsBase options) {
            var providerInfo = new QueryProviderInfo(source.Provider);
            var guardNulls = providerInfo.IsLinqToObjects;

#if DEBUG
            ExpressionWatcher = options.ExpressionWatcher;
            UseEnumerableOnce = options.UseEnumerableOnce;
            guardNulls = guardNulls && !options.SuppressGuardNulls;
#endif

            Source = source;
            Context = new DataSourceLoadContext(options, providerInfo, typeof(S));
            Builder = new DataSourceExpressionBuilder<S>(
                Context,
                guardNulls,
                new AnonTypeNewTweaks {
                    AllowEmpty = !providerInfo.IsL2S,
                    AllowUnusedMembers = !providerInfo.IsL2S
                }
            );
        }

        public LoadResult Load() {
            if(Context.IsCountQuery)
                return new LoadResult { totalCount = ExecCount() };

            var result = new LoadResult();

            if(Context.UseRemoteGrouping && Context.ShouldEmptyGroups) {
                var groupingResult = ExecRemoteGrouping();

                EmptyGroups(groupingResult.Groups, Context.Group.Count);

                result.data = Paginate(groupingResult.Groups, Context.Skip, Context.Take);
                result.summary = groupingResult.Totals;
                result.totalCount = groupingResult.TotalCount;

                if(Context.RequireGroupCount)
                    result.groupCount = groupingResult.Groups.Count();
            } else {
                var deferPaging = Context.HasGroups || !Context.UseRemoteGrouping && !Context.SummaryIsTotalCountOnly && Context.HasSummary;

                Expression loadExpr;

                if(!deferPaging && Context.PaginateViaPrimaryKey && Context.Skip > 0) {
                    if(!Context.HasPrimaryKey) {
                        throw new InvalidOperationException(nameof(DataSourceLoadOptionsBase.PaginateViaPrimaryKey)
                            + " requires a primary key."
                            + " Specify it via the " + nameof(DataSourceLoadOptionsBase.PrimaryKey) + " property.");
                    }

                    var loadKeysExpr = Builder.BuildLoadExpr(Source.Expression, true, selectOverride: Context.PrimaryKey);
                    var keyTuples = ExecExpr<AnonType>(Source, loadKeysExpr);

                    loadExpr = Builder.BuildLoadExpr(Source.Expression, false, filterOverride: FilterFromKeys(keyTuples));
                } else {
                    loadExpr = Builder.BuildLoadExpr(Source.Expression, !deferPaging);
                }

                if(Context.HasAnySelect) {
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
                    result.data = Paginate(result.data, Context.Skip, Context.Take);

                if(Context.ShouldEmptyGroups)
                    EmptyGroups(result.data, Context.Group.Count);
            }

            return result;
        }

        IEnumerable<ExpandoObject> ExecWithSelect(Expression loadExpr) {
            if(Context.UseRemoteSelect)
                return SelectHelper.ConvertRemoteResult(ExecExpr<AnonType>(Source, loadExpr), Context.FullSelect);

            return SelectHelper.Evaluate(ExecExpr<S>(Source, loadExpr), Context.FullSelect);
        }

        void ContinueWithGrouping<R>(IEnumerable<R> loadResult, LoadResult result) {
            var accessor = new DefaultAccessor<R>();
            if(Context.HasGroups) {
                var groups = new GroupHelper<R>(accessor).Group(loadResult, Context.Group);
                if(Context.RequireGroupCount)
                    result.groupCount = groups.Count;
                ContinueWithAggregation(groups, accessor, result);
            } else {
                ContinueWithAggregation(loadResult, accessor, result);
            }
        }

        void ContinueWithAggregation<R>(IEnumerable data, IAccessor<R> accessor, LoadResult result) {
            if(Context.UseRemoteGrouping && !Context.SummaryIsTotalCountOnly && Context.HasSummary && !Context.HasGroups) {
                var groupingResult = ExecRemoteGrouping();
                result.totalCount = groupingResult.TotalCount;
                result.summary = groupingResult.Totals;
            } else {
                var totalCount = -1;

                if(Context.RequireTotalCount || Context.SummaryIsTotalCountOnly)
                    totalCount = ExecCount();

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

        int ExecCount() {
            var expr = Builder.BuildCountExpr(Source.Expression);
#if DEBUG
            ExpressionWatcher?.Invoke(expr);
#endif
            return Source.Provider.Execute<int>(expr);
        }

        RemoteGroupingResult ExecRemoteGrouping() {
            return RemoteGroupTransformer.Run(
                typeof(S),
                ExecExpr<AnonType>(Source, Builder.BuildLoadGroupsExpr(Source.Expression)),
                Context.HasGroups ? Context.Group.Count : 0,
                Context.TotalSummary,
                Context.GroupSummary
            );
        }

        IEnumerable<R> ExecExpr<R>(IQueryable<S> source, Expression expr) {
            IEnumerable<R> result = source.Provider.CreateQuery<R>(expr);

#if DEBUG
            if(UseEnumerableOnce)
                result = new EnumerableOnce<R>(result);

            ExpressionWatcher?.Invoke(expr);
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
