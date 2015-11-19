using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    class DataSourceExpressionBuilder<T> {

        public int Skip { get; set; }
        public int Take { get; set; }
        public IList Filter { get; set; }
        public SortingInfo[] Sort { get; set; }

        public LambdaExpression Build(bool isCountQuery) {
            var queryableType = typeof(Queryable);
            var genericTypeArguments = new[] { typeof(T) };
            var paramExpr = Expression.Parameter(typeof(IQueryable<T>), "data");

            Expression body = paramExpr;

            if(Filter != null)
                body = Expression.Call(queryableType, "Where", genericTypeArguments, body, new FilterExpressionCompiler<T>().Compile(Filter));

            if(!isCountQuery) {
                if(Sort != null)
                    body = new SortExpressionCompiler<T>().Compile(body, Sort);

                if(Skip > 0)
                    body = Expression.Call(queryableType, "Skip", genericTypeArguments, body, Expression.Constant(Skip));

                if(Take > 0)
                    body = Expression.Call(queryableType, "Take", genericTypeArguments, body, Expression.Constant(Take));
            }

            if(isCountQuery)
                body = Expression.Call(queryableType, "Count", genericTypeArguments, body);

            return Expression.Lambda(body, paramExpr);
        }
    }

}
