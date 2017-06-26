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

        public Expression Compile(Expression target, string[] clientExprList) {
            var itemExpr = CreateItemParam(typeof(T));

            var accessors = clientExprList
                .Select(i => CompileAccessorExpression(itemExpr, i))
                .ToArray();

            var anonType = AnonType.Get(accessors.Select(i => i.Type).ToArray());

            return Expression.Call(typeof(Queryable), nameof(Queryable.Select), new[] { itemExpr.Type, anonType }, target, Expression.Quote(
                Expression.Lambda(
                    Expression.MemberInit(
                        Expression.New(anonType.GetConstructor(Type.EmptyTypes)),
                        accessors.Select((expr, i) => Expression.Bind(anonType.GetField(AnonType.ITEM_PREFIX + i), expr))
                    ),
                    itemExpr
                )
            ));
        }
    }

}
