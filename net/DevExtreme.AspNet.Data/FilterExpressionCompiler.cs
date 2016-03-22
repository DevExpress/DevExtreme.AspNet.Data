using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    class FilterExpressionCompiler<T> : ExpressionCompiler {

        public virtual LambdaExpression Compile(IList criteriaJson) {
            var dataItemExpr = Expression.Parameter(typeof(T), "obj");
            return Expression.Lambda(CompileCore(dataItemExpr, criteriaJson), dataItemExpr);
        }

        protected virtual Expression CompileCore(ParameterExpression dataItemExpr, IList criteriaJson) {
            if(IsCriteria(criteriaJson[0]))
                return CompileGroup(dataItemExpr, criteriaJson);

            return CompileBinary(dataItemExpr, criteriaJson);
        }

        protected virtual Expression CompileBinary(ParameterExpression dataItemExpr, IList criteriaJson) {
            var hasExplicitOperation = criteriaJson.Count > 2;

            var clientAccessor = Convert.ToString(criteriaJson[0]);
            var clientOperation = hasExplicitOperation ? Convert.ToString(criteriaJson[1]).ToLower() : "=";
            var clientValue = criteriaJson[hasExplicitOperation ? 2 : 1];

            var accessorExpr = CompileAccessorExpression(dataItemExpr, clientAccessor);

            if(IsStringFunction(clientOperation)) {
                if(accessorExpr.Type != typeof(String))
                    accessorExpr = ConvertToType(accessorExpr, typeof(String));

                return CompileStringFunction(accessorExpr, clientOperation, Convert.ToString(clientValue));

            } else {
                var expressionType = TranslateBinaryOperation(clientOperation);

                clientValue = Utils.ConvertClientValue(clientValue, accessorExpr.Type);
                Expression valueExpr = Expression.Constant(clientValue);

                if(clientValue is DateTime)
                    valueExpr = Workaround_3361((DateTime)clientValue);

                if(accessorExpr.Type != null && clientValue != null && clientValue.GetType() != accessorExpr.Type)
                    valueExpr = ConvertToType(valueExpr, accessorExpr.Type);

                return Expression.MakeBinary(expressionType, accessorExpr, valueExpr);
            }

        }

#warning remove when https://github.com/aspnet/EntityFramework/issues/3361 is fixed
        Expression Workaround_3361(DateTime date) {
            Expression<Func<DateTime>> closure = () => date;
            return closure.Body;
        }

        protected virtual bool IsStringFunction(string clientOperation) {
            return clientOperation == "contains" || clientOperation == "notcontains" || clientOperation == "startswith" || clientOperation == "endswith";
        }

        protected virtual Expression CompileStringFunction(Expression accessorExpr, string clientOperation, string value) {
            if(value != null)
                value = value.ToLower();

            var invert = false;

            if(clientOperation == "notcontains") {
                clientOperation = "contains";
                invert = true;
            }

            var toLowerMethod = typeof(String).GetMethod("ToLower", Type.EmptyTypes);
            var operationMethod = typeof(String)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Single(m => m.Name.Equals(clientOperation, StringComparison.OrdinalIgnoreCase) && m.GetParameters().Length == 1);

            Expression result = Expression.Call(
                Expression.Call(accessorExpr, toLowerMethod),
                operationMethod,
                Expression.Constant(value)
            );

            if(invert)
                result = Expression.Not(result);

            return result;
        }

        protected virtual Expression CompileGroup(ParameterExpression dataItemExpr, IList criteriaJson) {
            var operands = new List<Expression>();
            var isAnd = true;
            var nextIsAnd = true;

            foreach(var item in criteriaJson) {
                var operandJson = item as IList;

                if(IsCriteria(operandJson)) {
                    if(operands.Count > 1 && isAnd != nextIsAnd)
                        throw new ArgumentException("Mixing of and/or is not allowed inside a single group");

                    isAnd = nextIsAnd;
                    operands.Add(CompileCore(dataItemExpr, operandJson));
                    nextIsAnd = true;
                } else {
                    nextIsAnd = Regex.IsMatch(Convert.ToString(item), "and|&", RegexOptions.IgnoreCase);
                }
            }

            Expression result = null;
            var op = isAnd ? ExpressionType.AndAlso : ExpressionType.OrElse;

            foreach(var operand in operands) {
                if(result == null)
                    result = operand;
                else
                    result = Expression.MakeBinary(op, result, operand);
            }

            return result;
        }

        protected virtual ExpressionType TranslateBinaryOperation(string clientOperation) {
            switch(clientOperation) {
                case "=":
                    return ExpressionType.Equal;

                case "<>":
                    return ExpressionType.NotEqual;

                case ">":
                    return ExpressionType.GreaterThan;

                case ">=":
                    return ExpressionType.GreaterThanOrEqual;

                case "<":
                    return ExpressionType.LessThan;

                case "<=":
                    return ExpressionType.LessThanOrEqual;
            }

            throw new NotSupportedException();
        }

        protected virtual bool IsCriteria(object item) {
            return item is IList && !(item is String);
        }

    }

}
