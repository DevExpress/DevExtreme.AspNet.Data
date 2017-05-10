using DevExtreme.AspNet.Data.Helpers;
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

        public FilterExpressionCompiler(bool guardNulls)
            : base(guardNulls) {
        }

        public LambdaExpression Compile(IList criteriaJson) {
            var dataItemExpr = CreateItemParam(typeof(T));
            return Expression.Lambda(CompileCore(dataItemExpr, criteriaJson), dataItemExpr);
        }

        Expression CompileCore(ParameterExpression dataItemExpr, IList criteriaJson) {
            if(IsCriteria(criteriaJson[0]))
                return CompileGroup(dataItemExpr, criteriaJson);

            try {
                if(IsUnary(criteriaJson)) {
                    return CompileUnary(dataItemExpr, criteriaJson);
                }

                return CompileBinary(dataItemExpr, criteriaJson);
            } catch {
                return Expression.Constant(false);
            }
        }

        Expression CompileBinary(ParameterExpression dataItemExpr, IList criteriaJson) {
            var hasExplicitOperation = criteriaJson.Count > 2;

            var clientAccessor = Convert.ToString(criteriaJson[0]);
            var clientOperation = hasExplicitOperation ? Convert.ToString(criteriaJson[1]).ToLower() : "=";
            var clientValue = criteriaJson[hasExplicitOperation ? 2 : 1];
            var isStringOperation = clientOperation == "contains" || clientOperation == "notcontains" || clientOperation == "startswith" || clientOperation == "endswith";

            var accessorExpr = CompileAccessorExpression(dataItemExpr, clientAccessor, isStringOperation);

            if(isStringOperation) {
                return CompileStringFunction(accessorExpr, clientOperation, Convert.ToString(clientValue));

            } else {
                var expressionType = TranslateBinaryOperation(clientOperation);

                clientValue = Utils.ConvertClientValue(clientValue, accessorExpr.Type);
                Expression valueExpr = Expression.Constant(clientValue);

                if(accessorExpr.Type != null && clientValue != null && clientValue.GetType() != accessorExpr.Type)
                    valueExpr = Expression.Convert(valueExpr, accessorExpr.Type);

                if(accessorExpr.Type == typeof(String) && IsInequality(expressionType)) {
                    if(clientValue == null)
                        valueExpr = Expression.Constant(null, typeof(String));

                    var compareMethod = typeof(String).GetMethod("Compare", new[] { typeof(String), typeof(String) });
                    accessorExpr = Expression.Call(null, compareMethod, accessorExpr, valueExpr);
                    valueExpr = Expression.Constant(0);
                }

                return Expression.MakeBinary(expressionType, accessorExpr, valueExpr);
            }

        }

        bool IsInequality(ExpressionType type) {
            return type == ExpressionType.LessThan || type == ExpressionType.LessThanOrEqual || type == ExpressionType.GreaterThanOrEqual || type == ExpressionType.GreaterThan;
        }

        Expression CompileStringFunction(Expression accessorExpr, string clientOperation, string value) {
            if(value != null)
                value = value.ToLower();

            var invert = false;

            if(clientOperation == "notcontains") {
                clientOperation = "contains";
                invert = true;
            }

            if(GuardNulls)
                accessorExpr = Expression.Coalesce(accessorExpr, Expression.Constant(""));

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

        Expression CompileGroup(ParameterExpression dataItemExpr, IList criteriaJson) {
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

        Expression CompileUnary(ParameterExpression dataItemExpr, IList criteriaJson) {
            return Expression.Not(CompileCore(dataItemExpr, (IList)criteriaJson[1]));
        }

        ExpressionType TranslateBinaryOperation(string clientOperation) {
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

        bool IsCriteria(object item) {
            return item is IList && !(item is String);
        }

        internal bool IsUnary(IList criteriaJson) {
            return Convert.ToString(criteriaJson[0]) == "!";
        }

    }

}
