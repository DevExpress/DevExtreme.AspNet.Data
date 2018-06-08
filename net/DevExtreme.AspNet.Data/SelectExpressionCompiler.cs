using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DevExtreme.AspNet.Data {

    class SelectExpressionCompiler<T> : ExpressionCompiler {

        public SelectExpressionCompiler(bool guardNulls)
            : base(guardNulls) {
        }

        public Expression Compile(Expression target, IEnumerable<string> clientExprList) {
            var itemExpr = CreateItemParam(typeof(T));
            var lambda = Expression.Lambda(
                AnonType.CreateNewExpression(clientExprList.Select(i => CompileAccessorExpression(itemExpr, i, liftToNullable: true))),
                itemExpr
            );

            return Expression.Call(typeof(Queryable), nameof(Queryable.Select), new[] { itemExpr.Type, lambda.ReturnType }, target, Expression.Quote(lambda));
        }
    }

}
