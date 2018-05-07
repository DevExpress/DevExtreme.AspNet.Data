using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.RemoteGrouping {

    class RemoteGroupExpressionCompiler<T> : ExpressionCompiler {
        IEnumerable<GroupingInfo> _grouping;
        IEnumerable<SummaryInfo>
            _totalSummary,
            _groupSummary;

        public RemoteGroupExpressionCompiler(bool guardNulls, IEnumerable<GroupingInfo> grouping, IEnumerable<SummaryInfo> totalSummary, IEnumerable<SummaryInfo> groupSummary)
            : base(guardNulls) {
            _grouping = grouping;
            _totalSummary = totalSummary;
            _groupSummary = groupSummary;
        }

#if DEBUG
        public RemoteGroupExpressionCompiler(IEnumerable<GroupingInfo> grouping, IEnumerable<SummaryInfo> totalSummary, IEnumerable<SummaryInfo> groupSummary)
            : this(false, grouping, totalSummary, groupSummary) {
        }
#endif

        public Expression Compile(Expression target) {
            var groupByParam = CreateItemParam(typeof(T));
            var groupKeyExprList = new List<Expression>();
            var descendingList = new List<bool>();

            if(_grouping != null) {
                foreach(var i in _grouping) {
                    var selectorExpr = CompileAccessorExpression(groupByParam, i.Selector, liftToNullable: true);
                    if(!String.IsNullOrEmpty(i.GroupInterval))
                        selectorExpr = CompileGroupInterval(selectorExpr, i.GroupInterval);

                    groupKeyExprList.Add(selectorExpr);
                    descendingList.Add(i.Desc);
                }
            }

            var groupKeySelectorTypes = groupKeyExprList.Select(i => i.Type).ToArray();
            var groupKeyType = AnonType.Get(groupKeySelectorTypes);

            var groupKeyProps = Enumerable.Range(0, groupKeyExprList.Count)
                .Select(i => groupKeyType.GetField(AnonType.ITEM_PREFIX + i))
                .ToArray();

            var groupKeyLambda = Expression.Lambda(
                Expression.New(
                    groupKeyType.GetConstructor(groupKeySelectorTypes),
                    groupKeyExprList,
                    groupKeyProps
                ),
                groupByParam
            );

            var groupingType = typeof(IGrouping<,>).MakeGenericType(groupKeyType, typeof(T));

            target = Expression.Call(typeof(Queryable), nameof(Queryable.GroupBy), new[] { typeof(T), groupKeyType }, target, Expression.Quote(groupKeyLambda));

            for(var i = 0; i < groupKeyProps.Length; i++) {
                var orderParam = Expression.Parameter(groupingType, "g");
                var orderAccessor = Expression.Field(
                    Expression.Property(orderParam, "Key"),
                    groupKeyProps[i].Name
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
                Expression.Call(typeof(Enumerable), nameof(Enumerable.Count), new[] { typeof(T) }, param)
            };

            for(var i = 0; i < groupCount; i++)
                projectionExprList.Add(Expression.Field(Expression.Property(param, "Key"), AnonType.ITEM_PREFIX + i));

            projectionExprList.AddRange(MakeAggregates(param, _totalSummary));

            if(groupCount > 0)
                projectionExprList.AddRange(MakeAggregates(param, _groupSummary));

            var projectionType = AnonType.Get(projectionExprList.Select(i => i.Type).ToArray());

            var projectionLambda = Expression.Lambda(
                Expression.MemberInit(
                    Expression.New(projectionType.GetConstructor(Type.EmptyTypes)),
                    projectionExprList.Select((expr, i) => Expression.Bind(projectionType.GetField(AnonType.ITEM_PREFIX + i), expr))
                ),
                param
            );

            return Expression.Call(typeof(Queryable), nameof(Queryable.Select), new[] { param.Type, projectionType }, target, Expression.Quote(projectionLambda));
        }

        IEnumerable<Expression> MakeAggregates(Expression aggregateTarget, IEnumerable<SummaryInfo> summary) {
            foreach(var s in TransformSummary(summary)) {
                var itemParam = CreateItemParam(typeof(T));
                var selectorExpr = CompileAccessorExpression(itemParam, s.Selector, liftToNullable: true);
                var selectorType = selectorExpr.Type;

                var callType = typeof(Enumerable);
                var callMethod = GetPreAggregateMethodName(s.SummaryType);
                var callMethodTypeParams = new List<Type> { typeof(T) };
                var callArgs = new List<Expression> { aggregateTarget };

                if(s.SummaryType == AggregateName.COUNT_NOT_NULL) {
                    if(Utils.CanAssignNull(selectorType)) {
                        callArgs.Add(Expression.Lambda(
                            Expression.NotEqual(selectorExpr, Expression.Constant(null, selectorType)),
                            itemParam
                        ));
                    }
                } else {
                    if(!IsWellKnownAggregateDataType(selectorType)) {
                        if(s.SummaryType == AggregateName.MIN || s.SummaryType == AggregateName.MAX) {
                            callMethodTypeParams.Add(selectorType);
                        } else if(s.SummaryType == AggregateName.SUM) {
                            if(DynamicBindingHelper.ShouldUseDynamicBinding(typeof(T))) {
                                callType = typeof(DynamicSum);
                                callMethod = nameof(DynamicSum.Calculate);
                            } else {
                                selectorExpr = Expression.Convert(selectorExpr, GetSumType(selectorType));
                            }
                        }
                    }
                    callArgs.Add(Expression.Lambda(selectorExpr, itemParam));
                }

                yield return Expression.Call(callType, callMethod, callMethodTypeParams.ToArray(), callArgs.ToArray());
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

        static Type GetSumType(Type type) {
            var nullable = Utils.IsNullable(type);
            type = Utils.StripNullableType(type);

            if(type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort)) {
                type = typeof(int);
            } else if(type == typeof(uint)) {
                type = typeof(long);
            } else {
                type = typeof(decimal);
            }

            if(nullable)
                type = typeof(Nullable<>).MakeGenericType(type);

            return type;
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

            throw new NotSupportedException();
        }

        Expression CompileGroupInterval(Expression selectorExpr, string groupInterval) {
            var groupIntervalExpr = CompileGroupIntervalCore(selectorExpr, groupInterval);

            if(Utils.CanAssignNull(selectorExpr.Type)) {
                var nullableType = typeof(Nullable<>).MakeGenericType(groupIntervalExpr.Type);
                var nullConst = Expression.Constant(null, nullableType);

                return Expression.Condition(
                    Expression.Equal(selectorExpr, nullConst),
                    nullConst,
                    Expression.Convert(groupIntervalExpr, nullableType)
                );
            }

            return groupIntervalExpr;
        }

        Expression CompileGroupIntervalCore(Expression selector, string intervalString) {
            if(Char.IsDigit(intervalString[0])) {
                var intervalExpr = Expression.Constant(
                    Utils.ConvertClientValue(intervalString, selector.Type),
                    selector.Type
                );

                return Expression.MakeBinary(
                    ExpressionType.Subtract,
                    selector,
                    Expression.MakeBinary(ExpressionType.Modulo, selector, intervalExpr)
                );
            }

            switch(intervalString) {
                case "year":
                    return CompileAccessorExpression(selector, nameof(DateTime.Year));
                case "quarter":
                    return Expression.MakeBinary(
                        ExpressionType.Divide,
                        Expression.MakeBinary(
                            ExpressionType.Add,
                            CompileAccessorExpression(selector, nameof(DateTime.Month)),
                            Expression.Constant(2)
                        ),
                        Expression.Constant(3)
                    );
                case "month":
                    return CompileAccessorExpression(selector, nameof(DateTime.Month));
                case "day":
                    return CompileAccessorExpression(selector, nameof(DateTime.Day));
                case "dayOfWeek":
                    return Expression.Convert(CompileAccessorExpression(selector, nameof(DateTime.DayOfWeek)), typeof(int));
                case "hour":
                    return CompileAccessorExpression(selector, nameof(DateTime.Hour));
                case "minute":
                    return CompileAccessorExpression(selector, nameof(DateTime.Minute));
                case "second":
                    return CompileAccessorExpression(selector, nameof(DateTime.Second));
            }

            throw new NotSupportedException();
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
