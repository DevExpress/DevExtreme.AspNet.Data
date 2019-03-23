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

        public Expression Compile(Expression outer, Expression inner, IReadOnlyList<string> key) {
            if(key.Count < 2)
                return CompileForSingleKey(outer, inner, key);

            return CompileForMultiKey(outer, inner, key);
        }

        Expression CompileForSingleKey(Expression outer, Expression inner, IReadOnlyList<string> key) {
            var keyLambda = CompileKeyLambda(key);
            var whereParam = CreateItemParam(typeof(T));

            var selectCall = Expression.Call(
                typeof(Queryable),
                nameof(Queryable.Select),
                new[] { typeof(T), keyLambda.ReturnType },
                inner, Expression.Quote(keyLambda)
            );

            var containsCall = Expression.Call(
                typeof(Queryable),
                nameof(Queryable.Contains),
                new[] { keyLambda.ReturnType },
                selectCall,
                CompileAccessorExpression(whereParam, key[0])
            );

            return Expression.Call(
                typeof(Queryable),
                nameof(Queryable.Where),
                new[] { typeof(T) },
                outer,
                Expression.Quote(Expression.Lambda(containsCall, whereParam))
            );
        }

        Expression CompileForMultiKey(Expression outer, Expression inner, IReadOnlyList<string> key) {
            var keyLambda = CompileKeyLambda(key);
            var resultOuterParam = Expression.Parameter(typeof(T), "outer");
            var resultInnerParam = Expression.Parameter(typeof(T), "inner");
            var resultLambda = Expression.Lambda(resultOuterParam, resultOuterParam, resultInnerParam);

            return Expression.Call(
                typeof(Queryable),
                nameof(Queryable.Join),
                new[] { typeof(T), typeof(T), keyLambda.ReturnType, typeof(T) },
                outer, inner,
                Expression.Quote(keyLambda), Expression.Quote(keyLambda),
                Expression.Quote(resultLambda)
            );
        }

        LambdaExpression CompileKeyLambda(IReadOnlyList<string> key) {
            var param = CreateItemParam(typeof(T));
            return Expression.Lambda(
                key.Count < 2
                    ? CompileAccessorExpression(param, key[0])
                    : AnonType.CreateNewExpression(
                        key.Select(i => CompileAccessorExpression(param, i)).ToArray(),
                        _anonTypeNewTweaks
                    ),
                param
            );
        }

    }

}
