using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.RemoteGrouping {

    class RemoteGroupExpressionCompiler<T> : ExpressionCompiler {
        const int MAX_GROUPS = 8;
        const int MAX_AGGREGATES = 8;

        ParameterExpression _groupByParam;

        IList<Expression>
            _groupKeyExprList = new List<Expression>(),
            _totalSummaryExprList = new List<Expression>(),
            _groupSummaryExprList = new List<Expression>();

        IList<string>
            _totalSummaryTypes = new List<string>(),
            _groupSummaryTypes = new List<string>();

        IList<ParameterExpression>
            _totalSummaryParams = new List<ParameterExpression>(),
            _groupSummaryParams = new List<ParameterExpression>();

        Type _remoteGroupClassType;

        public RemoteGroupExpressionCompiler(GroupingInfo[] grouping, SummaryInfo[] totalSummary, SummaryInfo[] groupSummary) 
            : base(false) {

            _groupByParam = CreateItemParam(typeof(T));

            if(grouping != null) {
                foreach(var i in grouping) {
                    var selectorExpr = CompileAccessorExpression(_groupByParam, i.Selector);
                    if(!String.IsNullOrEmpty(i.GroupInterval))
                        selectorExpr = CompileGroupInterval(selectorExpr, i.GroupInterval);
                    _groupKeyExprList.Add(selectorExpr);
                }
            }

            if(totalSummary != null)
                InitSummary(totalSummary, _totalSummaryExprList, _totalSummaryTypes, _totalSummaryParams);

            if(_groupKeyExprList.Any() && groupSummary != null)
                InitSummary(groupSummary, _groupSummaryExprList, _groupSummaryTypes, _groupSummaryParams);

            if(_groupKeyExprList.Count > MAX_GROUPS)
                throw new NotSupportedException($"Maximum number of groups ({MAX_GROUPS}) exceeded");

            if(_totalSummaryExprList.Count > MAX_AGGREGATES)
                throw new NotSupportedException($"Maximum number of total-summary aggregates ({MAX_AGGREGATES}) exceeded");

            if(_groupSummaryExprList.Count > MAX_AGGREGATES)
                throw new NotSupportedException($"Maximum number of group-summary aggregates ({MAX_AGGREGATES}) exceeded");

            var typeArguments = new List<Type>(MAX_GROUPS + 2 * MAX_AGGREGATES);

            for(var i = 0; i < MAX_GROUPS; i++)
                typeArguments.Add(i < _groupKeyExprList.Count ? _groupKeyExprList[i].Type : typeof(Object));

            for(var i = 0; i < MAX_AGGREGATES; i++)
                typeArguments.Add(i < _totalSummaryExprList.Count ? _totalSummaryExprList[i].Type : typeof(Object));

            for(var i = 0; i < MAX_AGGREGATES; i++)
                typeArguments.Add(i < _groupSummaryExprList.Count ? _groupSummaryExprList[i].Type : typeof(Object));
            
            _remoteGroupClassType = typeof(RemoteGroup<,,,,,,,,,,,,,,,,,,,,,,,>).MakeGenericType(typeArguments.ToArray());
        }

        void InitSummary(IEnumerable<SummaryInfo> summary, IList<Expression> exprList, IList<string> summaryTypes, IList<ParameterExpression> paramList) {
            foreach(var i in summary) {
                var p = CreateItemParam(typeof(T));
                if(i.SummaryType == "count") {
                    exprList.Add(Expression.Constant(null));
                } else {
                    exprList.Add(CompileAccessorExpression(p, i.Selector));
                }
                summaryTypes.Add(i.SummaryType);
                paramList.Add(p);
            }
        }

        public Expression Compile(Expression target) {
            var groupKeySelectorTypes = _groupKeyExprList.Select(i => i.Type).ToArray();

            var groupKeyTypeArguments = new Type[MAX_GROUPS];
            for(var i = 0; i < MAX_GROUPS; i++)
                groupKeyTypeArguments[i] = i < groupKeySelectorTypes.Length ? groupKeySelectorTypes[i] : typeof(Object);

            var groupKeyType = typeof(RemoteGroupKey<,,,,,,,>).MakeGenericType(groupKeyTypeArguments);
            var groupKeyProps = GroupKeyNames().Select(i => groupKeyType.GetTypeInfo().GetDeclaredProperty(i)).ToArray();

            var groupKeyLambda = Expression.Lambda(
                Expression.New(
                    groupKeyType.GetConstructor(groupKeySelectorTypes),
                    _groupKeyExprList,
                    groupKeyProps
                ),
                _groupByParam
            );

            target = Expression.Call(typeof(Queryable), nameof(Queryable.GroupBy), new[] { typeof(T), groupKeyType }, target, groupKeyLambda);
            target = MakeAggregatingProjection(target, Expression.Parameter(typeof(IGrouping<,>).MakeGenericType(groupKeyType, typeof(T)), "g"));

            return target;
        }

        IEnumerable<string> GroupKeyNames() {
            for(var i = 0; i < _groupKeyExprList.Count; i++)
                yield return "K" + i;
        }

        Expression MakeAggregatingProjection(Expression target, ParameterExpression param) {
            var projectionBindings = new List<MemberAssignment>();

            foreach(var name in GroupKeyNames()) {
                projectionBindings.Add(Expression.Bind(
                    _remoteGroupClassType.GetTypeInfo().GetDeclaredField(name),
                    Expression.Property(Expression.Property(param, "Key"), name)
                ));
            }

            projectionBindings.Add(Expression.Bind(
                _remoteGroupClassType.GetTypeInfo().GetDeclaredProperty("Count"),
                Expression.Call(typeof(Enumerable), nameof(Enumerable.Count), new[] { typeof(T) }, param)
            ));

            AddAggregateBindings(projectionBindings, param, _groupSummaryExprList, _groupSummaryParams, _groupSummaryTypes, "G");
            AddAggregateBindings(projectionBindings, param, _totalSummaryExprList, _totalSummaryParams, _totalSummaryTypes, "T");

            var projectionLambda = Expression.Lambda(
                Expression.MemberInit(
                    Expression.New(_remoteGroupClassType.GetConstructor(Type.EmptyTypes)),
                    projectionBindings
                ),
                param
            );

            return Expression.Call(typeof(Queryable), "Select", new[] { param.Type, _remoteGroupClassType }, target, projectionLambda);
        }

        void AddAggregateBindings(ICollection<MemberAssignment> bindingList, Expression aggregateTarget, IList<Expression> selectorExprList, IList<ParameterExpression> summaryParams, IList<string> summaryTypes, string bindingFieldPrefix) {
            for(var i = 0; i < selectorExprList.Count; i++) {
                var summaryType = summaryTypes[i];

                if(summaryType == "count")
                    continue;

                bindingList.Add(
                    Expression.Bind(
                        _remoteGroupClassType.GetTypeInfo().GetDeclaredField(bindingFieldPrefix + i),
                        Expression.Call(
                            typeof(Enumerable),
                            GetPreAggregateMethodName(summaryType),
                            new[] { typeof(T) },
                            aggregateTarget,
                            Expression.Lambda(selectorExprList[i], summaryParams[i])
                        )
                    )
                );
            }
        }
        
        static string GetPreAggregateMethodName(string summaryType) {
            switch(summaryType) {
                case "min":
                    return nameof(Enumerable.Min);
                case "max":
                    return nameof(Enumerable.Max);
                case "sum":
                case "avg":
                    return nameof(Enumerable.Sum);
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


    }

}
