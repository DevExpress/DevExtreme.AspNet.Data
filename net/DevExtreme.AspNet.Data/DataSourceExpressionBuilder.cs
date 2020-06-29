using DevExtreme.AspNet.Data.RemoteGrouping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data {

    class DataSourceExpressionBuilder {
        Expression Expr;
        readonly DataSourceLoadContext Context;

        public DataSourceExpressionBuilder(Expression expr, DataSourceLoadContext context) {
            Expr = expr;
            Context = context;
        }

        public Expression BuildLoadExpr(bool paginate, IList filterOverride = null, IReadOnlyList<string> selectOverride = null) {
            AddFilter(filterOverride);
            AddSort();
            AddSelect(selectOverride);
            if(paginate)
                AddPaging();
            return Expr;
        }

        public Expression BuildCountExpr() {
            AddFilter();
            AddCount();
            return Expr;
        }

        public Expression BuildLoadGroupsExpr(bool paginate, bool suppressGroups = false, bool suppressTotals = false) {
            AddFilter();
            AddRemoteGrouping(suppressGroups, suppressTotals);
            if(paginate)
                AddPaging();
            return Expr;
        }

        public Expression BuildGroupCountExpr() {
            AddFilter();
            Expr = CreateSelectCompiler().CompileSingle(Expr, Context.Group.Single().Selector);
            Expr = QueryableCall(nameof(Queryable.Distinct));
            AddCount();
            return Expr;
        }

        void AddFilter(IList filterOverride = null) {
            if(filterOverride != null || Context.HasFilter) {
                var filterExpr = filterOverride != null && filterOverride.Count < 1
                    ? Expression.Lambda(Expression.Constant(false), Expression.Parameter(GetItemType()))
                    : new FilterExpressionCompiler(GetItemType(), Context.GuardNulls, Context.UseStringToLower).Compile(filterOverride ?? Context.Filter);

                Expr = QueryableCall(nameof(Queryable.Where), Expression.Quote(filterExpr));
            }
        }

        void AddSort() {
            if(Context.HasAnySort)
                Expr = new SortExpressionCompiler(GetItemType(), Context.GuardNulls).Compile(Expr, Context.GetFullSort());
        }

        void AddSelect(IReadOnlyList<string> selectOverride = null) {
            if(selectOverride != null || Context.HasAnySelect && Context.UseRemoteSelect)
                Expr = CreateSelectCompiler().Compile(Expr, selectOverride ?? Context.FullSelect);
        }

        void AddPaging() {
            if(Context.Skip > 0)
                Expr = QueryableCall(nameof(Queryable.Skip), Expression.Constant(Context.Skip));

            if(Context.Take > 0)
                Expr = QueryableCall(nameof(Queryable.Take), Expression.Constant(Context.Take));
        }

        void AddRemoteGrouping(bool suppressGroups, bool suppressTotals) {
            var compiler = new RemoteGroupExpressionCompiler(
                GetItemType(), Context.GuardNulls, Context.ExpandLinqSumType, Context.CreateAnonTypeNewTweaks(),
                suppressGroups ? null : Context.Group,
                suppressTotals ? null : Context.TotalSummary,
                suppressGroups ? null : Context.GroupSummary
            );
            Expr = compiler.Compile(Expr);
        }

        void AddCount() {
            Expr = QueryableCall(nameof(Queryable.Count));
        }

        SelectExpressionCompiler CreateSelectCompiler()
            => new SelectExpressionCompiler(GetItemType(), Context.GuardNulls, Context.CreateAnonTypeNewTweaks());

        Expression QueryableCall(string methodName)
            => Expression.Call(typeof(Queryable), methodName, GetQueryableGenericArguments(), Expr);

        Expression QueryableCall(string methodName, Expression arg)
            => Expression.Call(typeof(Queryable), methodName, GetQueryableGenericArguments(), Expr, arg);

        Type[] GetQueryableGenericArguments() {
            const string queryable1 = "IQueryable`1";
            var type = Expr.Type;

            if(type.IsInterface && type.Name == queryable1)
                return type.GenericTypeArguments;

            return type.GetInterface(queryable1).GenericTypeArguments;
        }

        Type GetItemType()
            => GetQueryableGenericArguments().First();

    }

}
