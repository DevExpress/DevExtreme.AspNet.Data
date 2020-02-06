using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.RemoteGrouping {

    class RemoteGroupExpressionCompiler : ExpressionCompiler {
        bool _expandSumType;
        AnonTypeNewTweaks _anonTypeNewTweaks;
        IEnumerable<GroupingInfo> _grouping;
        IEnumerable<SummaryInfo>
            _totalSummary,
            _groupSummary;

        public RemoteGroupExpressionCompiler(Type itemType, bool guardNulls, bool expandSumType, AnonTypeNewTweaks anonTypeNewTweaks, IEnumerable<GroupingInfo> grouping, IEnumerable<SummaryInfo> totalSummary, IEnumerable<SummaryInfo> groupSummary)
            : base(itemType, guardNulls) {
            _expandSumType = expandSumType;
            _anonTypeNewTweaks = anonTypeNewTweaks;
            _grouping = grouping;
            _totalSummary = totalSummary;
            _groupSummary = groupSummary;
        }

#if DEBUG
        public RemoteGroupExpressionCompiler(Type itemType, IEnumerable<GroupingInfo> grouping, IEnumerable<SummaryInfo> totalSummary, IEnumerable<SummaryInfo> groupSummary)
            : this(itemType, false, false, null, grouping, totalSummary, groupSummary) {
        }
#endif

        public Expression Compile(Expression target) {
            var groupByParam = CreateItemParam();
            var groupKeyExprList = new List<Expression>();
            var descendingList = new List<bool>();

            if(_grouping != null) {
                foreach(var i in _grouping) {
                    var selectorExpr = String.IsNullOrEmpty(i.GroupInterval)
                        ? CompileAccessorExpression(groupByParam, i.Selector, liftToNullable: true)
                        : CompileGroupInterval(groupByParam, i.Selector, i.GroupInterval);

                    groupKeyExprList.Add(selectorExpr);
                    descendingList.Add(i.Desc);
                }
            }

            var groupKeyLambda = Expression.Lambda(AnonType.CreateNewExpression(groupKeyExprList, _anonTypeNewTweaks), groupByParam);
            var groupingType = typeof(IGrouping<,>).MakeGenericType(groupKeyLambda.ReturnType, ItemType);

            target = Expression.Call(typeof(Queryable), nameof(Queryable.GroupBy), new[] { ItemType, groupKeyLambda.ReturnType }, target, Expression.Quote(groupKeyLambda));

            for(var i = 0; i < groupKeyExprList.Count; i++) {
                var orderParam = Expression.Parameter(groupingType, "g");
                var orderAccessor = Expression.Field(
                    Expression.Property(orderParam, "Key"),
                    AnonType.IndexToField(i)
                );

                target = Expression.Call(
                    typeof(Queryable),
                    Utils.GetSortMethod(i == 0, descendingList[i]),
                    new[] { groupingType, orderAccessor.Type },
                    target,
                    Expression.Quote(Expression.Lambda(orderAccessor, orderParam))
                );
            }

            return MakeAggregatingProjection(target, groupingType, groupKeyExprList.Count);
        }

        Expression MakeAggregatingProjection(Expression target, Type groupingType, int groupCount) {
            var param = Expression.Parameter(groupingType, "g");

            var projectionExprList = new List<Expression> {
                Expression.Call(typeof(Enumerable), nameof(Enumerable.Count), new[] { ItemType }, param)
            };

            for(var i = 0; i < groupCount; i++)
                projectionExprList.Add(Expression.Field(Expression.Property(param, "Key"), AnonType.IndexToField(i)));

            projectionExprList.AddRange(MakeAggregates(param, _totalSummary));

            if(groupCount > 0)
                projectionExprList.AddRange(MakeAggregates(param, _groupSummary));

            var projectionLambda = Expression.Lambda(AnonType.CreateNewExpression(projectionExprList, _anonTypeNewTweaks), param);

            return Expression.Call(typeof(Queryable), nameof(Queryable.Select), new[] { param.Type, projectionLambda.ReturnType }, target, Expression.Quote(projectionLambda));
        }

        IEnumerable<Expression> MakeAggregates(Expression aggregateTarget, IEnumerable<SummaryInfo> summary) {
            foreach(var s in TransformSummary(summary)) {
                yield return MakeAggregate(aggregateTarget, s);
            }
        }

        Expression MakeAggregate(Expression aggregateTarget, SummaryInfo s) {
            var itemParam = CreateItemParam();
            var selectorExpr = CompileAccessorExpression(itemParam, s.Selector, liftToNullable: true);
            var selectorType = selectorExpr.Type;

            var callType = typeof(Enumerable);
            var isCountNotNull = s.SummaryType == AggregateName.COUNT_NOT_NULL;

            if(isCountNotNull && Utils.CanAssignNull(selectorType)) {
                return Expression.Call(
                    callType,
                    nameof(Enumerable.Sum),
                    Type.EmptyTypes,
                    Expression.Call(
                        typeof(Enumerable),
                        nameof(Enumerable.Select),
                        new[] { ItemType, typeof(int) },
                        aggregateTarget,
                        Expression.Lambda(
                            Expression.Condition(
                                Expression.NotEqual(selectorExpr, Expression.Constant(null, selectorType)),
                                Expression.Constant(1),
                                Expression.Constant(0)
                            ),
                            itemParam
                        )
                    )
                );
            } else {
                var callMethod = GetPreAggregateMethodName(s.SummaryType);
                var callMethodTypeParams = new List<Type> { ItemType };
                var callArgs = new List<Expression> { aggregateTarget };

                try {
                    if(s.SummaryType == AggregateName.MIN || s.SummaryType == AggregateName.MAX) {
                        if(!IsWellKnownAggregateDataType(selectorType))
                            callMethodTypeParams.Add(selectorType);
                    } else if(s.SummaryType == AggregateName.SUM) {
                        if(DynamicBindingHelper.ShouldUseDynamicBinding(ItemType)) {
                            callType = typeof(DynamicSum);
                            callMethod = nameof(DynamicSum.Calculate);
                        } else {
                            selectorExpr = ConvertSumSelector(selectorExpr);
                        }
                    }

                    if(!isCountNotNull)
                        callArgs.Add(Expression.Lambda(selectorExpr, itemParam));

                    return Expression.Call(callType, callMethod, callMethodTypeParams.ToArray(), callArgs.ToArray());
                } catch(Exception x) {
                    var message = $"Failed to translate the '{s.SummaryType}' aggregate for the '{s.Selector}' member ({selectorExpr.Type}). See InnerException for details.";
                    throw new Exception(message, x);
                }
            }
        }

        static bool IsWellKnownAggregateDataType(Type type) {
            type = Utils.StripNullableType(type);
            return type == typeof(decimal)
                || type == typeof(double)
                || type == typeof(float)
                || type == typeof(int)
                || type == typeof(long);
        }

        Expression ConvertSumSelector(Expression expr) {
            var type = expr.Type;
            var nullable = Utils.IsNullable(type);

            if(nullable)
                type = Utils.StripNullableType(type);

            var sumType = GetSumType(type, _expandSumType);
            if(sumType == type)
                return expr;

            if(nullable)
                sumType = Utils.MakeNullable(sumType);

            return Expression.Convert(expr, sumType);
        }

        internal static Type GetSumType(Type type, bool expand) {
            if(type == typeof(decimal) || type == typeof(double) || type == typeof(long))
                return type;

            if(type == typeof(int) || type == typeof(byte) || type == typeof(short) || type == typeof(sbyte) || type == typeof(ushort))
                return expand ? typeof(long) : typeof(int);

            if(type == typeof(float))
                return expand ? typeof(double) : typeof(float);

            if(type == typeof(uint))
                return typeof(long);

            return typeof(decimal);
        }

        static string GetPreAggregateMethodName(string summaryType) {
            switch(summaryType) {
                case AggregateName.MIN:
                    return nameof(Enumerable.Min);
                case AggregateName.MAX:
                    return nameof(Enumerable.Max);
                case AggregateName.SUM:
                    return nameof(Enumerable.Sum);
                case AggregateName.COUNT_NOT_NULL:
                    return nameof(Enumerable.Count);
            }

            if(CustomAggregators.IsRegistered(summaryType)) {
                var message = $"The custom aggregate '{summaryType}' cannot be translated to a LINQ expression."
                    + $" Set {nameof(DataSourceLoadOptionsBase)}.{nameof(DataSourceLoadOptionsBase.RemoteGrouping)} to False to enable in-memory aggregate calculation.";
                throw new InvalidOperationException(message);
            }

            throw new NotSupportedException();
        }

        Expression CompileGroupInterval(Expression target, string selector, string intervalString) {
            if(Char.IsDigit(intervalString[0]))
                return CompileNumericGroupInterval(target, selector, intervalString);

            return CompileDateGroupInterval(target, selector, intervalString);
        }

        Expression CompileNumericGroupInterval(Expression target, string selector, string intervalString) {
            return CompileAccessorExpression(
                target,
                selector,
                progression => {
                    var lastIndex = progression.Count - 1;
                    var last = progression[lastIndex];

                    var intervalExpr = Expression.Constant(
                        Utils.ConvertClientValue(intervalString, last.Type),
                        last.Type
                    );

                    progression[lastIndex] = Expression.MakeBinary(
                        ExpressionType.Subtract,
                        last,
                        Expression.MakeBinary(ExpressionType.Modulo, last, intervalExpr)
                    );
                },
                true
            );
        }

        Expression CompileDateGroupInterval(Expression target, string selector, string intervalString) {
            return CompileAccessorExpression(
                target,
                selector + "." + (intervalString == "quarter" ? "month" : intervalString),
                progression => {
                    var lastIndex = progression.Count - 1;
                    var last = progression[lastIndex];

                    if(intervalString == "quarter") {
                        progression[lastIndex] = Expression.MakeBinary(
                            ExpressionType.Divide,
                            Expression.MakeBinary(ExpressionType.Add, last, Expression.Constant(2)),
                            Expression.Constant(3)
                        );
                    } else if(intervalString == "dayOfWeek") {
                        var hasNullable = progression.Any(i => Utils.CanAssignNull(i.Type));
                        progression[lastIndex] = Expression.Convert(last, hasNullable ? typeof(int?) : typeof(int));
                    }
                },
                true
            );
        }

        static IEnumerable<SummaryInfo> TransformSummary(IEnumerable<SummaryInfo> source) {
            if(source == null)
                yield break;

            foreach(var i in source) {
                if(i.SummaryType == AggregateName.COUNT)
                    continue;
                if(i.SummaryType == AggregateName.AVG) {
                    yield return new SummaryInfo { Selector = i.Selector, SummaryType = AggregateName.SUM };
                    yield return new SummaryInfo { Selector = i.Selector, SummaryType = AggregateName.COUNT_NOT_NULL };
                } else {
                    yield return i;
                }
            }
        }

    }

}
