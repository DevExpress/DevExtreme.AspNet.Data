using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    abstract class ExpressionCompiler {

        protected virtual bool GuardNulls {
            get { return false; }
        }

        protected internal Expression CompileAccessorExpression_NEW(Expression target, string clientExpr) {
            if(clientExpr == "this")
                return target;

            var components = clientExpr.Split('.');
            var componentExpressions = new List<Expression>(1 + components.Length);
            componentExpressions.Add(target);

            var currentTarget = target;
            foreach(var i in components) {
                Expression next = Expression.PropertyOrField(currentTarget, i);

                if(GuardNulls && next.Type == typeof(String))
                    next = Expression.Coalesce(next, Expression.Constant(""));

                currentTarget = next;
                componentExpressions.Add(next);
            }

            var lastComponent = componentExpressions.Last();

            if(!GuardNulls)
                return lastComponent;

            var resultType = lastComponent.Type;
            Expression guardCondition = null;
            foreach(var i in componentExpressions) {
                if(i == lastComponent)
                    break;

                var type = i.Type;
                if(type == typeof(String) || !Utils.CanAssignNull(type))
                    break;

                var componentCondition = Expression.Equal(i, Expression.Constant(null, type));
                if(guardCondition == null)
                    guardCondition = componentCondition;
                else
                    guardCondition = Expression.OrElse(guardCondition, componentCondition);
            }

            return Expression.Condition(
                guardCondition,
                Expression.Constant(Utils.GetDefaultValue(lastComponent.Type), lastComponent.Type),
                lastComponent
            );
        }


        protected Expression CompileAccessorExpression(Expression target, string clientExpr) {
            if(clientExpr == "this")
                return target;

            var lastDotIndex = clientExpr.LastIndexOf('.');
            if(lastDotIndex > -1) {
                target = CompileAccessorExpression(target, clientExpr.Substring(0, lastDotIndex));
                clientExpr = clientExpr.Substring(1 + lastDotIndex);
            }

            return Expression.PropertyOrField(target, clientExpr);
        }

        protected Expression ConvertToType(Expression expr, Type type) {
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
