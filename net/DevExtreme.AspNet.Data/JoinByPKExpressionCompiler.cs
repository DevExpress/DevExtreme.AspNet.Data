using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data {

    class JoinByPKExpressionCompiler<T> : ExpressionCompiler {
        AnonTypeNewTweaks _anonTypeNewTweaks;

        public JoinByPKExpressionCompiler(bool guardNulls, AnonTypeNewTweaks anonTypeNewTweaks)
            : base(guardNulls) {
            _anonTypeNewTweaks = anonTypeNewTweaks;
        }

        public Expression Compile(Expression outer, Expression inner, IReadOnlyList<string> pk) {
            var dataItemExpr = CreateItemParam(typeof(T));

            var pkParam = CreateItemParam(typeof(T));
            var pkLambda = Expression.Lambda(
                pk.Count < 2
                    ? CompileAccessorExpression(pkParam, pk[0])
                    : AnonType.CreateNewExpression(
                        pk.Select(i => CompileAccessorExpression(pkParam, i)).ToArray(),
                        _anonTypeNewTweaks
                    ),
                pkParam
            );

            var resultOuterParam = Expression.Parameter(typeof(T), "outer");
            var resultInnerParam = Expression.Parameter(typeof(T), "inner");
            var resultLambda = Expression.Lambda(resultOuterParam, resultOuterParam, resultInnerParam);

            return Expression.Call(
                typeof(Queryable),
                nameof(Queryable.Join),
                new[] { typeof(T), typeof(T), pkLambda.ReturnType, typeof(T) },
                outer, inner,
                Expression.Quote(pkLambda), Expression.Quote(pkLambda),
                Expression.Quote(resultLambda)
            );
        }

    }

}
