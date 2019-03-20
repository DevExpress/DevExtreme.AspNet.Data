using DevExtreme.AspNet.Data.RemoteGrouping;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data {

    class DataSourceExpressionBuilder<T> {
        DataSourceLoadContext _context;
        bool _guardNulls;
        AnonTypeNewTweaks _anonTypeNewTweaks;

        public DataSourceExpressionBuilder(DataSourceLoadContext context, bool guardNulls = false, AnonTypeNewTweaks anonTypeNewTweaks = null) {
            _context = context;
            _guardNulls = guardNulls;
            _anonTypeNewTweaks = anonTypeNewTweaks;
        }

        public Expression BuildLoadExpr(Expression source, bool paginate = true, IList filterOverride = null, IReadOnlyList<string> selectOverride = null) {
            return BuildCore(source, paginate: paginate, filterOverride: filterOverride, selectOverride: selectOverride);
        }

        public Expression BuildCountExpr(Expression source) {
            return BuildCore(source, isCountQuery: true);
        }

        public Expression BuildLoadGroupsExpr(Expression source) {
            return BuildCore(source, remoteGrouping: true);
        }

        Expression BuildCore(Expression expr, bool paginate = false, bool isCountQuery = false, bool remoteGrouping = false, IList filterOverride = null, IReadOnlyList<string> selectOverride = null) {
            var queryableType = typeof(Queryable);
            var genericTypeArguments = new[] { typeof(T) };

            if(filterOverride != null || _context.HasFilter)
                expr = Expression.Call(queryableType, "Where", genericTypeArguments, expr, Expression.Quote(new FilterExpressionCompiler<T>(_guardNulls, _context.UseStringToLower).Compile(filterOverride ?? _context.Filter)));

            if(!isCountQuery) {
                if(!remoteGrouping) {
                    if(_context.HasAnySort)
                        expr = new SortExpressionCompiler<T>(_guardNulls).Compile(expr, _context.GetFullSort());
                    if(selectOverride != null || _context.HasAnySelect && _context.UseRemoteSelect) {
                        expr = new SelectExpressionCompiler<T>(_guardNulls).Compile(expr, selectOverride ?? _context.FullSelect);
                        genericTypeArguments = expr.Type.GetGenericArguments();
                    }
                } else {
                    expr = new RemoteGroupExpressionCompiler<T>(_guardNulls, _anonTypeNewTweaks, _context.Group, _context.TotalSummary, _context.GroupSummary).Compile(expr);
                }

                if(paginate) {
                    if(_context.Skip > 0)
                        expr = Expression.Call(queryableType, "Skip", genericTypeArguments, expr, Expression.Constant(_context.Skip));

                    if(_context.Take > 0)
                        expr = Expression.Call(queryableType, "Take", genericTypeArguments, expr, Expression.Constant(_context.Take));
                }
            }

            if(isCountQuery)
                expr = Expression.Call(queryableType, "Count", genericTypeArguments, expr);

            return expr;
        }
    }

}
