using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    abstract class ExpressionCompiler {
        protected readonly Type ItemType;
        protected readonly bool GuardNulls;

        public ExpressionCompiler(Type itemType, bool guardNulls) {
            ItemType = itemType;
            GuardNulls = guardNulls;
        }

        protected internal Expression CompileAccessorExpression(Expression target, string clientExpr, Action<List<Expression>> customizeProgression = null, bool liftToNullable = false) {
            var customResult = CustomAccessorCompilers.TryCompile(target, clientExpr);
            if(customResult != null)
                return customResult;

            var progression = new List<Expression> { target };

            var clientExprItems = clientExpr.Split('.');
            var currentTarget = target;

            for(var i = 0; i < clientExprItems.Length; i++) {
                var clientExprItem = clientExprItems[i];

                if(i == 0 && clientExprItem == "this")
                    continue;

                if(Utils.IsNullable(currentTarget.Type)) {
                    clientExprItem = "Value";
                    i--;
                }

                if(currentTarget.Type == typeof(ExpandoObject))
                    currentTarget = ReadExpando(currentTarget, clientExprItem);
                else if(DynamicBindingHelper.ShouldUseDynamicBinding(currentTarget.Type))
                    currentTarget = DynamicBindingHelper.CompileGetMember(currentTarget, clientExprItem);
                else
                    currentTarget = FixReflectedType(Expression.PropertyOrField(currentTarget, clientExprItem));

                progression.Add(currentTarget);
            }

            customizeProgression?.Invoke(progression);

            if(GuardNulls && progression.Count > 1 || liftToNullable && progression.Count > 2) {
                var lastIndex = progression.Count - 1;
                var last = progression[lastIndex];
                if(Utils.CanAssignNull(target.Type) && !Utils.CanAssignNull(last.Type))
                    progression[lastIndex] = Expression.Convert(last, Utils.MakeNullable(last.Type));
            }

            return CompileNullGuard(progression);
        }

        Expression CompileNullGuard(IEnumerable<Expression> progression) {
            var last = progression.Last();
            var lastType = last.Type;

            if(!GuardNulls)
                return last;

            Expression allTests = null;

            foreach(var i in progression) {
                if(i == last)
                    break;

                var type = i.Type;
                if(!Utils.CanAssignNull(type))
                    continue;

                var test = Expression.Equal(i, Expression.Constant(null, type));
                if(allTests == null)
                    allTests = test;
                else
                    allTests = Expression.OrElse(allTests, test);
            }

            if(allTests == null)
                return last;

            return Expression.Condition(
                allTests,
                Expression.Constant(Utils.GetDefaultValue(lastType), lastType),
                last
            );
        }

        protected ParameterExpression CreateItemParam() {
            return Expression.Parameter(ItemType, "obj");
        }

        internal static void ForceToString(List<Expression> progression) {
            var last = progression.Last();
            if(last.Type != typeof(String))
                progression.Add(Expression.Call(last, typeof(Object).GetMethod(nameof(Object.ToString))));
        }

        static Expression ReadExpando(Expression expando, string member) {
            return Expression.Property(
                Expression.Convert(expando, typeof(IDictionary<string, object>)),
                "Item",
                Expression.Constant(member)
            );
        }

        static MemberExpression FixReflectedType(MemberExpression expr) {
            var member = expr.Member;
            var declaringType = member.DeclaringType;

            if(member.ReflectedType != declaringType) {
                switch(member.MemberType) {
                    case MemberTypes.Property:
                        return Expression.Property(expr.Expression, declaringType, member.Name);
                    case MemberTypes.Field:
                        return Expression.Field(expr.Expression, declaringType, member.Name);
                }
            }

            return expr;
        }
    }

}
