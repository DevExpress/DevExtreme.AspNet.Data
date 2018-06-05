using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DevExtreme.AspNet.Data {

    class SelectExpressionCompiler<T> : ExpressionCompiler {

        public SelectExpressionCompiler(bool guardNulls)
            : base(guardNulls) {
        }

        public Expression Compile(Expression target, IEnumerable<string> clientExprList) {
            var itemExpr = CreateItemParam(typeof(T));

            var accessors = clientExprList
                .Select(i => CompileAccessorExpression(itemExpr, i, liftToNullable: true))
                .ToArray();

            var tupleType = TupleUtils.CreateType(accessors.Select(i => i.Type).ToArray());

            return Expression.Call(typeof(Queryable), nameof(Queryable.Select), new[] { itemExpr.Type, tupleType }, target, Expression.Quote(
                Expression.Lambda(
                    TupleUtils.CreateNewExpr(tupleType, accessors),
                    itemExpr
                )
            ));
        }
    }

}
