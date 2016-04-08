using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    abstract class ExpressionCompiler {

        protected virtual Expression CompileAccessorExpression(Expression target, string clientExpr) {
            if(clientExpr == "this")
                return target;

            var lastDotIndex = clientExpr.LastIndexOf('.');
            if(lastDotIndex > -1) {
                target = CompileAccessorExpression(target, clientExpr.Substring(0, lastDotIndex));
                clientExpr = clientExpr.Substring(1 + lastDotIndex);
            }

            return Expression.PropertyOrField(target, clientExpr);
        }

        protected virtual Expression ConvertToType(Expression expr, Type type) {
            if(type == typeof(String)) {
                var result = Expression.Call(expr, "ToString", Type.EmptyTypes);

                if(!Utils.CanAssignNull(expr.Type))
                    return result;

                return Expression.Condition(
                    Expression.Equal(expr, Expression.Constant(null)),
                    Expression.Constant(String.Empty),
                    result
                );
            }

            return Expression.Convert(expr, type);
        }

        protected ParameterExpression CreateItemParam(Type type) {
            return Expression.Parameter(type, "obj");
        }

    }

}
