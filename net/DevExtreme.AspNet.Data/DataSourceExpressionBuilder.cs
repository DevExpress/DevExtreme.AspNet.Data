using DevExtreme.AspNet.Data.RemoteGrouping;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    class DataSourceExpressionBuilder<T> {
        DataSourceLoadOptionsBase _loadOptions;
        bool _guardNulls;

        public DataSourceExpressionBuilder(DataSourceLoadOptionsBase loadOptions, bool guardNulls) {
            _loadOptions = loadOptions;
            _guardNulls = guardNulls;
        }

        public Expression BuildLoadExpr(Expression source, bool paginate = true) {
            return BuildCore(source, paginate: paginate);
        }

        public Expression BuildCountExpr(Expression source) {
            return BuildCore(source, isCountQuery: true);
        }

        public Expression BuildLoadGroupsExpr(Expression source) {
            return BuildCore(source, remoteGrouping: true);
        }

        Expression BuildCore(Expression expr, bool paginate = false, bool isCountQuery = false, bool remoteGrouping = false) {
            var queryableType = typeof(Queryable);
            var genericTypeArguments = new[] { typeof(T) };

            if(_loadOptions.HasFilter)
                expr = Expression.Call(queryableType, "Where", genericTypeArguments, expr, Expression.Quote(new FilterExpressionCompiler<T>(_guardNulls).Compile(_loadOptions.Filter)));

            if(!isCountQuery) {
                if(!remoteGrouping) {
                    if(_loadOptions.HasAnySort)
                        expr = new SortExpressionCompiler<T>(_guardNulls).Compile(expr, _loadOptions.GetFullSort());
                    if(_loadOptions.HasSelect) {
                        expr = new SelectExpressionCompiler<T>(_guardNulls).Compile(expr, _loadOptions.Select);
                        genericTypeArguments = expr.Type.GetGenericArguments();
                    }
                } else {
                    expr = new RemoteGroupExpressionCompiler<T>(_loadOptions.Group, _loadOptions.TotalSummary, _loadOptions.GroupSummary).Compile(expr);
                }

                if(paginate) {
                    if(_loadOptions.Skip > 0)
                        expr = Expression.Call(queryableType, "Skip", genericTypeArguments, expr, Expression.Constant(_loadOptions.Skip));

                    if(_loadOptions.Take > 0)
                        expr = Expression.Call(queryableType, "Take", genericTypeArguments, expr, Expression.Constant(_loadOptions.Take));
                }
            }

            if(isCountQuery)
                expr = Expression.Call(queryableType, "Count", genericTypeArguments, expr);

            return expr;
        }
    }

}
