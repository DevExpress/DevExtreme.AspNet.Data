using DevExtreme.AspNet.Data.Aggregation.Accumulators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Aggregation {

    // Rationale: normalization across LINQ providers
    // https://github.com/aspnet/EntityFrameworkCore/issues/12307
    // https://data.uservoice.com/forums/72025/suggestions/2410716
    // https://dba.stackexchange.com/q/25435
    // https://en.wikipedia.org/wiki/Empty_sum

    class SumFix : ExpressionCompiler {
        Expression _typeParam;
        IList<SummaryInfo> _totalSummary;
        IList<SummaryInfo> _groupSummary;
        IDictionary<string, object> _defaultValues;

        public SumFix(Type type, IList<SummaryInfo> totalSummary, IList<SummaryInfo> groupSummary)
            : base(false) {
            _typeParam = Expression.Parameter(type);
            _totalSummary = totalSummary;
            _groupSummary = groupSummary;
        }

        public void ApplyToTotal(object[] values) {
            Apply(_totalSummary, values);
        }

        public void ApplyToGroup(object[] values) {
            Apply(_groupSummary, values);
        }

        void Apply(IList<SummaryInfo> summary, object[] values) {
            if(summary == null)
                return;

            for(var i = 0; i < summary.Count; i++) {
                if(values[i] != null)
                    continue;

                var summaryItem = summary[i];
                if(summaryItem.SummaryType != AggregateName.SUM)
                    continue;

                values[i] = GetDefaultValue(summaryItem.Selector);
            }
        }

        object GetDefaultValue(string selector) {
            if(_defaultValues == null)
                _defaultValues = new Dictionary<string, object>();

            if(!_defaultValues.ContainsKey(selector)) {
                var expr = CompileAccessorExpression(_typeParam, selector);
                var acc = AccumulatorFactory.Create(Utils.StripNullableType(expr.Type));
                _defaultValues[selector] = acc.GetValue();
            }

            return _defaultValues[selector];
        }
    }

}
