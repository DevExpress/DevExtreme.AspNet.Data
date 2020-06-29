using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DevExtreme.AspNet.Data {

    class SelectExpressionCompiler : ExpressionCompiler {
        AnonTypeNewTweaks _anonTypeNewTweaks;

        public SelectExpressionCompiler(Type itemType, bool guardNulls, AnonTypeNewTweaks anonTypeNewTweaks = null)
            : base(itemType, guardNulls) {
            _anonTypeNewTweaks = anonTypeNewTweaks;
        }

        public Expression Compile(Expression target, IEnumerable<string> clientExprList)
            => Compile(target, clientExprList, true);

        public Expression CompileSingle(Expression target, string clientExpr)
            => Compile(target, new[] { clientExpr }, false);

        Expression Compile(Expression target, IEnumerable<string> clientExprList, bool useNew) {
            var itemExpr = CreateItemParam();

            var memberExprList = clientExprList
                .Select(i => CompileAccessorExpression(itemExpr, i, liftToNullable: true))
                .ToArray();

            var lambda = Expression.Lambda(
                useNew
                    ? AnonType.CreateNewExpression(memberExprList, _anonTypeNewTweaks)
                    : memberExprList[0],
                itemExpr
            );

            return Expression.Call(typeof(Queryable), nameof(Queryable.Select), new[] { itemExpr.Type, lambda.ReturnType }, target, Expression.Quote(lambda));
        }
    }

}
