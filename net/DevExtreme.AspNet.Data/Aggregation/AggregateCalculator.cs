using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class AggregateCalculator<T> {
        IEnumerable<object> _data;
        Accessor<T> _accessor;

        Aggregator[] _totalAggregators;
        string[] _totalSelectors;

        string[] _groupSummaryTypes;
        string[] _groupSelectors;
        Stack<Aggregator[]> _groupAggregatorsStack;


        public AggregateCalculator(IEnumerable<object> data, Accessor<T> accessor, IEnumerable<SummaryInfo> totalSummary, IEnumerable<SummaryInfo> groupSummary) {
            _data = data;
            _accessor = accessor;

            if(totalSummary != null) {
                _totalAggregators = totalSummary.Select(i => CreateAggregator(i.SummaryType)).ToArray();
                _totalSelectors = totalSummary.Select(i => i.Selector).ToArray();
            }

            if(groupSummary != null) {
                _groupSummaryTypes = groupSummary.Select(i => i.SummaryType).ToArray();
                _groupSelectors = groupSummary.Select(i => i.Selector).ToArray();
                _groupAggregatorsStack = new Stack<Aggregator[]>();
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
            var group = item as DevExtremeGroup;
            if(group != null) {
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

        void ProcessGroup(DevExtremeGroup group) {
            if(_groupAggregatorsStack != null)
                _groupAggregatorsStack.Push(_groupSummaryTypes.Select(CreateAggregator).ToArray());
            
            foreach(var i in group.items)
                ProcessItem(i);

            if(_groupAggregatorsStack != null)
                group.summary = Finish(_groupAggregatorsStack.Pop());            
        }

        void Step(object obj, Aggregator[] aggregators, string[] selectors) {
            var typed = (T)obj;
            for(var i = 0; i < aggregators.Length; i++)
                aggregators[i].Step(_accessor.Read(typed, selectors[i]));
        }

        object[] Finish(Aggregator[] aggregators) {
            return aggregators.Select(a => a.Finish()).ToArray();
        }


        Aggregator CreateAggregator(string summaryType) {
            switch(summaryType) {
                case "sum":
                    return new SumAggregator();
                case "min":
                    return new MinAggregator();
                case "max":
                    return new MaxAggregator();
                case "avg":
                    return new AvgAggregator();
                case "count":
                    return new CountAggregator(false);
            }

            throw new NotSupportedException();
        }

    }

}
