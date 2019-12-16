using DevExtreme.AspNet.Data.RemoteGrouping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data {

    class DataSourceExpressionBuilder<T> {
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

        public Expression BuildLoadGroupsExpr() {
            AddFilter();
            AddRemoteGrouping();
            return Expr;
        }

        void AddFilter(IList filterOverride = null) {
            if(filterOverride != null || Context.HasFilter) {
                var filterExpr = filterOverride != null && filterOverride.Count < 1
                    ? Expression.Lambda(Expression.Constant(false), Expression.Parameter(typeof(T)))
                    : new FilterExpressionCompiler<T>(Context.GuardNulls, Context.UseStringToLower).Compile(filterOverride ?? Context.Filter);

                Expr = Expression.Call(typeof(Queryable), "Where", new[] { typeof(T) }, Expr, Expression.Quote(filterExpr));
            }
        }

        void AddSort() {
            if(Context.HasAnySort)
                Expr = new SortExpressionCompiler<T>(Context.GuardNulls).Compile(Expr, Context.GetFullSort());
        }

        void AddSelect(IReadOnlyList<string> selectOverride = null) {
            if(selectOverride != null || Context.HasAnySelect && Context.UseRemoteSelect)
                Expr = new SelectExpressionCompiler<T>(Context.GuardNulls, Context.CreateAnonTypeNewTweaks()).Compile(Expr, selectOverride ?? Context.FullSelect);
        }

        void AddPaging() {
            var queryableType = typeof(Queryable);
            var genericTypeArguments = Expr.Type.GetGenericArguments();

            if(Context.Skip > 0)
                Expr = Expression.Call(queryableType, "Skip", genericTypeArguments, Expr, Expression.Constant(Context.Skip));

            if(Context.Take > 0)
                Expr = Expression.Call(queryableType, "Take", genericTypeArguments, Expr, Expression.Constant(Context.Take));
        }

        void AddRemoteGrouping() {
            var compiler = new RemoteGroupExpressionCompiler<T>(
                Context.GuardNulls, Context.ExpandLinqSumType, Context.CreateAnonTypeNewTweaks(),
                Context.Group, Context.TotalSummary, Context.GroupSummary
            );
            Expr = compiler.Compile(Expr);
        }

        void AddCount() {
            Expr = Expression.Call(typeof(Queryable), "Count", Expr.Type.GetGenericArguments(), Expr);
        }
    }

}
