using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    class SortExpressionCompiler<T> : ExpressionCompiler {

        public virtual Expression Compile(Expression target, SortingInfo[] clientExprList) {
            var dataItemExpr = Expression.Parameter(typeof(T), "obj");
            var first = true;

            foreach(var item in clientExprList) {
                var methodName = first
                    ? (item.Desc ? "OrderByDescending" : "OrderBy")
                    : (item.Desc ? "ThenByDescending" : "ThenBy");

                var accessorExpr = CompileAccessorExpression(dataItemExpr, item.Selector);

                target = Expression.Call(typeof(Queryable), methodName, new[] { typeof(T), accessorExpr.Type }, target, Expression.Lambda(accessorExpr, dataItemExpr));
                first = false;
            }

            return target;
        }


    }

}
