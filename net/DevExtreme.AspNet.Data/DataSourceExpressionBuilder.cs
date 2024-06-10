using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data
{

    class DataSourceExpressionBuilder
    {
        Expression Expr;
        readonly DataSourceLoadContext Context;

        public DataSourceExpressionBuilder(Expression expr, DataSourceLoadContext context)
        {
            Expr = expr;
            Context = context;
        }

        public Expression BuildLoadExpr(bool paginate, IList filterOverride = null, IReadOnlyList<string> selectOverride = null, Type projectionType = null)
        {
            if (Context.ProjectBeforeFilter)
            {
                AddSelect(selectOverride, projectionType);
                AddFilter(filterOverride);
                AddSort();
                if (paginate)
                    AddPaging();
                return Expr;
            }
            else
            {
                AddFilter(filterOverride);
                AddSort();
                AddSelect(selectOverride, projectionType);
                if (paginate)
                    AddPaging();
                return Expr;
            }
        }

        public Expression BuildCountExpr()
        {
            AddFilter();
            AddCount();
            return Expr;
        }

        public Expression BuildLoadGroupsExpr(bool paginate, bool suppressGroups = false, bool suppressTotals = false, Type projectionType = null)
        {
            if (Context.ProjectBeforeFilter)
            {
                AddSelect(null, projectionType);
            }
            AddFilter();
            AddRemoteGrouping(suppressGroups, suppressTotals);
            if (paginate)
                AddPaging();
            return Expr;
        }

        public Expression BuildGroupCountExpr()
        {
            AddFilter();
            Expr = CreateSelectCompiler().CompileSingle(Expr, Context.Group.Single().Selector);
            Expr = QueryableCall(nameof(Queryable.Distinct));
            AddCount();
            return Expr;
        }

        void AddFilter(IList filterOverride = null)
        {
            if (filterOverride != null || Context.HasFilter)
            {
                var filterExpr = filterOverride != null && filterOverride.Count < 1
                    ? Expression.Lambda(Expression.Constant(false), Expression.Parameter(GetItemType()))
                    : new FilterExpressionCompiler(GetItemType(), Context.GuardNulls, Context.UseStringToLower, Context.SupportsEqualsMethod, Context.AutomapperProjectionParameters).Compile(filterOverride ?? Context.Filter);

                Expr = QueryableCall(nameof(Queryable.Where), Expression.Quote(filterExpr));
            }
        }

        void AddSort()
        {
            if (Context.HasAnySort)
                Expr = new SortExpressionCompiler(GetItemType(), Context.GuardNulls, Context.AutomapperProjectionParameters).Compile(Expr, Context.GetFullSort());
        }

        void AddSelect(IReadOnlyList<string> selectOverride = null, Type projectionType = null)
        {
            if (selectOverride != null || Context.HasAnySelect && Context.UseRemoteSelect)
                Expr = CreateSelectCompiler().Compile(Expr, selectOverride ?? Context.FullSelect);
            else if (projectionType != null)
            {
                var _type = GetItemType();
                var qryBase = Context.Mapper.ConfigurationProvider.Internal().ProjectionBuilder.GetProjection(_type, projectionType, Context.AutomapperProjectionParameters, Array.Empty<MemberPath>());
                var qryExpr = qryBase.Projection;
                if (qryBase.LetClause != null && qryBase.Projection != null)
                {
                    //Automapper can multi-stage complex queries. Need to push one select into another.
                    var srcType = qryExpr.Parameters[0].Type;
                    Expr = Expression.Call(typeof(Queryable), nameof(Queryable.Select), new[] { _type, qryBase.LetClause.ReturnType }, Expr, qryBase.LetClause);
                    Expr = Expression.Call(typeof(Queryable), nameof(Queryable.Select), new[] { qryBase.LetClause.ReturnType, qryBase.Projection.ReturnType }, Expr, qryBase.Projection);
                }
                else
                {
                    Expr = Expression.Call(typeof(Queryable), nameof(Queryable.Select), new[] { _type, qryExpr.ReturnType }, Expr, Expression.Quote(qryExpr));
                }
            }
        }

        void AddPaging()
        {
            if (Context.Skip > 0)
                Expr = QueryableCall(nameof(Queryable.Skip), Expression.Constant(Context.Skip));

            if (Context.Take > 0)
                Expr = QueryableCall(nameof(Queryable.Take), Expression.Constant(Context.Take));
        }

        void AddRemoteGrouping(bool suppressGroups, bool suppressTotals)
        {
            var compiler = new RemoteGroupExpressionCompiler(
                GetItemType(), Context.GuardNulls, Context.ExpandLinqSumType, Context.CreateAnonTypeNewTweaks(),
                suppressGroups ? null : Context.Group,
                suppressTotals ? null : Context.TotalSummary,
                suppressGroups ? null : Context.GroupSummary,
                Context.AutomapperProjectionParameters
            );
            Expr = compiler.Compile(Expr);
        }

        void AddCount()
        {
            Expr = QueryableCall(nameof(Queryable.Count));
        }

        SelectExpressionCompiler CreateSelectCompiler()
            => new SelectExpressionCompiler(GetItemType(), Context.GuardNulls, Context.CreateAnonTypeNewTweaks(), Context.AutomapperProjectionParameters);

        Expression QueryableCall(string methodName)
            => Expression.Call(typeof(Queryable), methodName, GetQueryableGenericArguments(), Expr);

        Expression QueryableCall(string methodName, Expression arg)
            => Expression.Call(typeof(Queryable), methodName, GetQueryableGenericArguments(), Expr, arg);

        Type[] GetQueryableGenericArguments()
        {
            const string queryable1 = "IQueryable`1";
            var type = Expr.Type;

            if (type.IsInterface && type.Name == queryable1)
                return type.GenericTypeArguments;

            return type.GetInterface(queryable1).GenericTypeArguments;
        }

        Type GetItemType()
            => GetQueryableGenericArguments().First();

    }

}
