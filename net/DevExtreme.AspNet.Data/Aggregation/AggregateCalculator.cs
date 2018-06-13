using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Data.RemoteGrouping;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class AggregateCalculator<T> {
        IEnumerable _data;
        IAccessor<T> _accessor;
        SumFix _sumFix;
        IList<SummaryInfo> _totalSummary;
        IList<SummaryInfo> _groupSummary;

        Aggregator<T>[] _totalAggregators;
        Stack<Aggregator<T>[]> _groupAggregatorsStack;


        public AggregateCalculator(IEnumerable data, IAccessor<T> accessor, IList<SummaryInfo> totalSummary, IList<SummaryInfo> groupSummary, SumFix sumFix = null) {
            _data = data;
            _accessor = accessor;
            _totalSummary = totalSummary;
            _groupSummary = groupSummary;
            _sumFix = sumFix ?? new SumFix(typeof(T), totalSummary, groupSummary);

            _totalAggregators = _totalSummary?.Select(CreateAggregator).ToArray();

            if(groupSummary != null)
                _groupAggregatorsStack = new Stack<Aggregator<T>[]>();
        }

        public object[] Run() {
            foreach(var item in _data)
                ProcessItem(item);

            if(_totalAggregators != null) {
                var values = Finish(_totalAggregators);
                _sumFix.ApplyToTotal(values);
                return values;
            }

            return null;
        }

        void ProcessItem(object item) {
            if(item is Group group) {
                ProcessGroup(group);
            } else {
                if(_groupAggregatorsStack != null) {
                    foreach(var groupAggregators in _groupAggregatorsStack)
                        Step(item, groupAggregators, _groupSummary);
                }

                if(_totalAggregators != null)
                    Step(item, _totalAggregators, _totalSummary);
            }
        }

        void ProcessGroup(Group group) {
            if(_groupAggregatorsStack != null)
                _groupAggregatorsStack.Push(_groupSummary.Select(CreateAggregator).ToArray());

            foreach(var i in group.items)
                ProcessItem(i);

            if(_groupAggregatorsStack != null) {
                group.summary = Finish(_groupAggregatorsStack.Pop());
                _sumFix.ApplyToGroup(group.summary);
            }
        }

        void Step(object obj, Aggregator<T>[] aggregators, IList<SummaryInfo> summary) {
            var typed = (T)obj;
            for(var i = 0; i < aggregators.Length; i++)
                aggregators[i].Step(typed, summary[i].Selector);
        }

        object[] Finish(Aggregator<T>[] aggregators) {
            return aggregators.Select(a => a.Finish()).ToArray();
        }


        Aggregator<T> CreateAggregator(SummaryInfo summaryInfo) {
            var summaryType = summaryInfo.SummaryType;

            switch(summaryType) {
                case AggregateName.SUM:
                    return new SumAggregator<T>(_accessor);
                case AggregateName.MIN:
                    return new MinAggregator<T>(_accessor);
                case AggregateName.MAX:
                    return new MaxAggregator<T>(_accessor);
                case AggregateName.AVG:
                    return new AvgAggregator<T>(_accessor);
                case AggregateName.COUNT:
                    return new CountAggregator<T>(_accessor, false);

                case AggregateName.REMOTE_COUNT:
                    return new RemoteCountAggregator<T>(_accessor);
                case AggregateName.REMOTE_AVG:
                    return new RemoteAvgAggregator<T>(_accessor);
            }

            var aggregator = CustomAggregators.CreateAggregator(summaryType, _accessor);
            if(aggregator != null)
                return aggregator;

            throw new NotSupportedException();
        }

    }

}
