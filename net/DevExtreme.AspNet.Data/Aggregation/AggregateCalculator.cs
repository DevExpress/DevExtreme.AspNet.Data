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

        Aggregator<T>[] _totalAggregators;
        string[] _totalSelectors;

        string[] _groupSummaryTypes;
        string[] _groupSelectors;
        Stack<Aggregator<T>[]> _groupAggregatorsStack;


        public AggregateCalculator(IEnumerable data, IAccessor<T> accessor, IEnumerable<SummaryInfo> totalSummary, IEnumerable<SummaryInfo> groupSummary) {
            _data = data;
            _accessor = accessor;

            if(totalSummary != null) {
                _totalAggregators = totalSummary.Select(i => CreateAggregator(i.SummaryType)).ToArray();
                _totalSelectors = totalSummary.Select(i => i.Selector).ToArray();
            }

            if(groupSummary != null) {
                _groupSummaryTypes = groupSummary.Select(i => i.SummaryType).ToArray();
                _groupSelectors = groupSummary.Select(i => i.Selector).ToArray();
                _groupAggregatorsStack = new Stack<Aggregator<T>[]>();
            }
        }

        public object[] Run() {
            foreach(var item in _data)
                ProcessItem(item);

            if(_totalAggregators != null)
                return Finish(_totalAggregators);

            return null;
        }

        void ProcessItem(object item) {
            if(item is Group group) {
                ProcessGroup(group);
            } else {
                if(_groupAggregatorsStack != null) {
                    foreach(var groupAggregators in _groupAggregatorsStack)
                        Step(item, groupAggregators, _groupSelectors);
                }

                if(_totalAggregators != null)
                    Step(item, _totalAggregators, _totalSelectors);
            }
        }

        void ProcessGroup(Group group) {
            if(_groupAggregatorsStack != null)
                _groupAggregatorsStack.Push(_groupSummaryTypes.Select(CreateAggregator).ToArray());

            foreach(var i in group.items)
                ProcessItem(i);

            if(_groupAggregatorsStack != null)
                group.summary = Finish(_groupAggregatorsStack.Pop());
        }

        void Step(object obj, Aggregator<T>[] aggregators, string[] selectors) {
            var typed = (T)obj;
            for(var i = 0; i < aggregators.Length; i++)
                aggregators[i].Step(typed, selectors[i]);
        }

        object[] Finish(Aggregator<T>[] aggregators) {
            return aggregators.Select(a => a.Finish()).ToArray();
        }


        Aggregator<T> CreateAggregator(string summaryType) {
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
