using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Data.RemoteGrouping;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DevExtreme.AspNet.Data {

    class DataSourceLoaderImpl<S> {
        readonly IQueryable<S> Source;
        readonly DataSourceLoadOptionsBase Options;

        readonly DataSourceExpressionBuilder<S> Builder;
        readonly bool ShouldEmptyGroups;
        readonly bool CanUseRemoteGrouping;
        readonly bool SummaryIsTotalCountOnly;

        public DataSourceLoaderImpl(IQueryable<S> source, DataSourceLoadOptionsBase options) {
            var isLinqToObjects = source is EnumerableQuery;

            // Until https://github.com/aspnet/EntityFramework/issues/2341 is implemented
            // local grouping is more efficient for EF Core
            var preferLocalGrouping = Compat.IsEFCore(source.Provider);

            Builder = new DataSourceExpressionBuilder<S>(options, isLinqToObjects);
            ShouldEmptyGroups = options.HasGroups && !options.Group.Last().GetIsExpanded();
            CanUseRemoteGrouping = options.RemoteGrouping ?? !(isLinqToObjects || preferLocalGrouping);
            SummaryIsTotalCountOnly = !options.HasGroupSummary && options.HasSummary && options.TotalSummary.All(i => i.SummaryType == "count");

            Source = source;
            Options = options;
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

                if(!Options.HasPrimaryKey && (Options.Skip > 0 || Options.Take > 0) && Compat.IsEntityFramework(Source.Provider))
                    Options.DefaultSort = EFSorting.FindSortableMember(typeof(S));

                var deferPaging = Options.HasGroups || !CanUseRemoteGrouping && !SummaryIsTotalCountOnly && Options.HasSummary;
                var loadExpr = Builder.BuildLoadExpr(Source.Expression, !deferPaging);

                if(Options.HasSelect) {
                    ContinueWithGrouping(
                        ExecExpr<AnonType>(Source, loadExpr).Select(ProjectionToDict),
                        Accessors.Dict,
                        result
                    );
                } else {
                    ContinueWithGrouping(
                        ExecExpr<S>(Source, loadExpr),
                        new DefaultAccessor<S>(),
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

        void ContinueWithGrouping<R>(IEnumerable<R> loadResult, IAccessor<R> accessor, LoadResult result) {
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

        Dictionary<string, object> ProjectionToDict(AnonType projection) {
            var names = Options.Select;
            var dict = new Dictionary<string, object>();
            for(var i = 0; i < names.Length; i++)
                ShrinkSelectResult(dict, names[i].Split('.'), projection[i]);
            return dict;
        }

        static void ShrinkSelectResult(IDictionary<string, object> target, string[] path, object value, int index = 0) {
            var key = path[index];

            if(index == path.Length - 1) {
                target[key] = value;
            } else {
                if(!target.ContainsKey(key))
                    target[key] = new Dictionary<string, object>();

                var child = target[key] as IDictionary<string, object>;
                if(child != null)
                    ShrinkSelectResult(child, path, value, 1 + index);
            }
        }
    }

}
