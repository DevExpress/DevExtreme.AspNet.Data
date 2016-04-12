using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    class DataSourceExpressionBuilder<T> {
        DataSourceLoadOptionsBase _loadOptions;

        public DataSourceExpressionBuilder(DataSourceLoadOptionsBase loadOptions) {
            _loadOptions = loadOptions;
        }

        public Expression<Func<IQueryable<T>, IQueryable<T>>> BuildLoadExpr() {
            var param = CreateParam();
            return Expression.Lambda<Func<IQueryable<T>, IQueryable<T>>>(
                BuildCore(param, false),
                param
            );
        }

        public Expression<Func<IQueryable<T>, int>> BuildCountExpr() {
            var param = CreateParam();
            return Expression.Lambda<Func<IQueryable<T>, int>>(
                BuildCore(param, true),
                param
            );
        }

        Expression BuildCore(ParameterExpression param, bool isCountQuery) {
            var queryableType = typeof(Queryable);
            var genericTypeArguments = new[] { typeof(T) };

            Expression body = param;

            if(_loadOptions.Filter != null)
                body = Expression.Call(queryableType, "Where", genericTypeArguments, body, new FilterExpressionCompiler<T>().Compile(_loadOptions.Filter));

            if(!isCountQuery) {
                if(_loadOptions.HasSort || _loadOptions.HasGroups)
                    body = new SortExpressionCompiler<T>().Compile(body, _loadOptions.GetFullSort());

                if(!_loadOptions.HasGroups && !_loadOptions.HasSummary) {
                    if(_loadOptions.Skip > 0)
                        body = Expression.Call(queryableType, "Skip", genericTypeArguments, body, Expression.Constant(_loadOptions.Skip));

                    if(_loadOptions.Take > 0)
                        body = Expression.Call(queryableType, "Take", genericTypeArguments, body, Expression.Constant(_loadOptions.Take));
                }
            }

            if(isCountQuery)
                body = Expression.Call(queryableType, "Count", genericTypeArguments, body);

            return body;
        }


        ParameterExpression CreateParam() {
            return Expression.Parameter(typeof(IQueryable<T>), "data");
        }
    }

}
