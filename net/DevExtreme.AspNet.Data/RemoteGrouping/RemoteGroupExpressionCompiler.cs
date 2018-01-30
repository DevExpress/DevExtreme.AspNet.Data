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
        ParameterExpression _groupByParam;

        IList<Expression>
            _groupKeyExprList = new List<Expression>(),
            _totalSummaryExprList = new List<Expression>(),
            _groupSummaryExprList = new List<Expression>();

        IList<bool>
            _descendingList = new List<bool>();

        IList<string>
            _totalSummaryTypes = new List<string>(),
            _groupSummaryTypes = new List<string>();

        IList<ParameterExpression>
            _totalSummaryParams = new List<ParameterExpression>(),
            _groupSummaryParams = new List<ParameterExpression>();

        Type _remoteGroupType;

        public RemoteGroupExpressionCompiler(GroupingInfo[] grouping, SummaryInfo[] totalSummary, SummaryInfo[] groupSummary)
            : base(false) {

            totalSummary = TransformSummary(totalSummary);
            groupSummary = TransformSummary(groupSummary);

            _groupByParam = CreateItemParam(typeof(T));

            if(grouping != null) {
                foreach(var i in grouping) {
                    var selectorExpr = CompileAccessorExpression(_groupByParam, i.Selector);
                    if(!String.IsNullOrEmpty(i.GroupInterval)) {
                        var groupIntervalExpr = CompileGroupInterval(selectorExpr, i.GroupInterval);

                        if(Utils.CanAssignNull(selectorExpr.Type)) {
                            var nullableType = typeof(Nullable<>).MakeGenericType(groupIntervalExpr.Type);
                            var nullConst = Expression.Constant(null, nullableType);
                            var test = Expression.Equal(selectorExpr, nullConst);

                            groupIntervalExpr = Expression.Convert(groupIntervalExpr, nullableType);
                            selectorExpr = Expression.Condition(test, nullConst, groupIntervalExpr);
                        } else {
                            selectorExpr = groupIntervalExpr;
                        }
                    }

                    _groupKeyExprList.Add(selectorExpr);
                    _descendingList.Add(i.Desc);
                }
            }

            if(totalSummary != null)
                InitSummary(totalSummary, _totalSummaryExprList, _totalSummaryTypes, _totalSummaryParams);

            if(_groupKeyExprList.Any() && groupSummary != null)
                InitSummary(groupSummary, _groupSummaryExprList, _groupSummaryTypes, _groupSummaryParams);

            var typeArguments = new[] { typeof(int) }
                .Concat(_groupKeyExprList.Concat(_totalSummaryExprList).Concat(_groupSummaryExprList).Select(i => i.Type))
                .ToArray();

            _remoteGroupType = AnonType.Get(typeArguments);
        }

        void InitSummary(IEnumerable<SummaryInfo> summary, IList<Expression> exprList, IList<string> summaryTypes, IList<ParameterExpression> paramList) {
            foreach(var i in summary) {
                var p = CreateItemParam(typeof(T));
                exprList.Add(CompileAccessorExpression(p, i.Selector));
                summaryTypes.Add(i.SummaryType);
                paramList.Add(p);
            }
        }

        public Expression Compile(Expression target) {
            var groupKeySelectorTypes = _groupKeyExprList.Select(i => i.Type).ToArray();
            var groupKeyType = AnonType.Get(groupKeySelectorTypes);

            var groupKeyProps = Enumerable.Range(0, _groupKeyExprList.Count)
                .Select(i => groupKeyType.GetField(AnonType.ITEM_PREFIX + i))
                .ToArray();

            var groupKeyLambda = Expression.Lambda(
                Expression.New(
                    groupKeyType.GetConstructor(groupKeySelectorTypes),
                    _groupKeyExprList,
                    groupKeyProps
                ),
                _groupByParam
            );

            var groupingType = typeof(IGrouping<,>).MakeGenericType(groupKeyType, typeof(T));

            target = Expression.Call(typeof(Queryable), nameof(Queryable.GroupBy), new[] { typeof(T), groupKeyType }, target, Expression.Quote(groupKeyLambda));

            for(var i = 0; i < groupKeyProps.Length; i++) {
                var orderParam = Expression.Parameter(groupingType, "g");
                var orderAccessor = CompileAccessorExpression(orderParam, "Key." + groupKeyProps[i].Name);

                target = Expression.Call(
                    typeof(Queryable),
                    Utils.GetSortMethod(i == 0, _descendingList[i]),
                    new[] { groupingType, orderAccessor.Type },
                    target,
                    Expression.Quote(Expression.Lambda(orderAccessor, orderParam))
                );
            }

            target = MakeAggregatingProjection(target, Expression.Parameter(groupingType, "g"));

            return target;
        }

        Expression MakeAggregatingProjection(Expression target, ParameterExpression param) {
            var projectionBindings = new List<MemberAssignment> {
                Expression.Bind(
                    _remoteGroupType.GetField(AnonType.ITEM_PREFIX + 0),
                    Expression.Call(typeof(Enumerable), nameof(Enumerable.Count), new[] { typeof(T) }, param)
                )
            };

            for(var i = 0; i < _groupKeyExprList.Count; i++) {
                projectionBindings.Add(Expression.Bind(
                    _remoteGroupType.GetField(AnonType.ITEM_PREFIX + (1 + i)),
                    Expression.Field(Expression.Property(param, "Key"), AnonType.ITEM_PREFIX + i)
                ));
            }

            var bindingFieldIndex = 1 + _groupKeyExprList.Count;

            AddAggregateBindings(projectionBindings, param, _totalSummaryExprList, _totalSummaryParams, _totalSummaryTypes, ref bindingFieldIndex);
            AddAggregateBindings(projectionBindings, param, _groupSummaryExprList, _groupSummaryParams, _groupSummaryTypes, ref bindingFieldIndex);

            var projectionLambda = Expression.Lambda(
                Expression.MemberInit(
                    Expression.New(_remoteGroupType.GetConstructor(Type.EmptyTypes)),
                    projectionBindings
                ),
                param
            );

            return Expression.Call(typeof(Queryable), nameof(Queryable.Select), new[] { param.Type, _remoteGroupType }, target, Expression.Quote(projectionLambda));
        }

        void AddAggregateBindings(ICollection<MemberAssignment> bindingList, Expression aggregateTarget, IList<Expression> selectorExprList, IList<ParameterExpression> summaryParams, IList<string> summaryTypes, ref int bindingFieldIndex) {
            for(var i = 0; i < selectorExprList.Count; i++) {
                var summaryType = summaryTypes[i];
                var selectorExpr = selectorExprList[i];
                var callArgs = new List<Expression> { aggregateTarget };

                if(summaryType == AggregateName.COUNT_NOT_NULL) {
                    if(Utils.CanAssignNull(selectorExpr.Type)) {
                        callArgs.Add(
                            Expression.Lambda(
                                Expression.NotEqual(selectorExpr, Expression.Constant(null, selectorExpr.Type)),
                                summaryParams[i]
                            )
                        );
                    }
                } else {
                    callArgs.Add(Expression.Lambda(selectorExpr, summaryParams[i]));
                }

                bindingList.Add(
                    Expression.Bind(
                        _remoteGroupType.GetField(AnonType.ITEM_PREFIX + bindingFieldIndex),
                        Expression.Call(
                            typeof(Enumerable),
                            GetPreAggregateMethodName(summaryType),
                            new[] { typeof(T) },
                            callArgs.ToArray()
                        )
                    )
                );

                bindingFieldIndex++;
            }
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

        Expression CompileGroupInterval(Expression selector, string intervalString) {
            if(Char.IsDigit(intervalString[0])) {
                return Expression.MakeBinary(
                    ExpressionType.Subtract,
                    selector,
                    Expression.MakeBinary(
                        ExpressionType.Modulo,
                        selector,
                        Expression.Convert(
                            Expression.Constant(Int32.Parse(intervalString)),
                            selector.Type
                        )
                    )
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

        static SummaryInfo[] TransformSummary(IEnumerable<SummaryInfo> source) {
            if(source == null)
                return null;

            var result = new List<SummaryInfo>();
            foreach(var i in source) {
                if(i.SummaryType == AggregateName.COUNT)
                    continue;
                if(i.SummaryType == AggregateName.AVG) {
                    result.Add(new SummaryInfo { Selector = i.Selector, SummaryType = AggregateName.SUM });
                    result.Add(new SummaryInfo { Selector = i.Selector, SummaryType = AggregateName.COUNT_NOT_NULL });
                } else {
                    result.Add(i);
                }
            }

            return result.ToArray();
        }

    }

}
