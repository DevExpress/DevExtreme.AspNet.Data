using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    abstract class ExpressionCompiler {
        bool _guardNulls;

        public ExpressionCompiler(bool guardNulls) {
            _guardNulls = guardNulls;
        }

        protected internal Expression CompileAccessorExpression(Expression target, string clientExpr, bool forceToString = false) {
            if(clientExpr == "this")
                return target;

            var components = new List<Expression>();
            components.Add(target);

            var currentTarget = target;
            foreach(var i in clientExpr.Split('.')) {
                Expression next = Expression.PropertyOrField(currentTarget, i);

                if(_guardNulls && next.Type == typeof(String))
                    next = Expression.Coalesce(next, Expression.Constant(""));

                currentTarget = next;
                components.Add(next);
            }

            if(forceToString && currentTarget.Type != typeof(String))
                components.Add(Expression.Call(currentTarget, "ToString", Type.EmptyTypes));            
            
            return CompileNullGuard(components);
        }

        Expression CompileNullGuard(IEnumerable<Expression> components) {
            var last = components.Last();
            var lastType = last.Type;

            if(!_guardNulls)
                return last;

            Expression allTests = null;

            foreach(var i in components) {
                if(i == last)
                    break;

                var type = i.Type;
                if(type == typeof(String) || !Utils.CanAssignNull(type))
                    break;

                var test = Expression.Equal(i, Expression.Constant(null, type));
                if(allTests == null)
                    allTests = test;
                else
                    allTests = Expression.OrElse(allTests, test);
            }

            return Expression.Condition(
                allTests,
                Expression.Constant(
                    lastType == typeof(String) ? "" : Utils.GetDefaultValue(lastType), 
                    lastType
                ),
                last
            );
        }

        protected ParameterExpression CreateItemParam(Type type) {
            return Expression.Parameter(type, "obj");
        }

    }

}
