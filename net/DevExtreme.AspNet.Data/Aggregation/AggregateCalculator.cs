using DevExtreme.AspNet.Data.RemoteGrouping;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DevExtreme.AspNet.Data.Aggregation {

    public class AggregateCalculator<T> {
        IEnumerable _data;
        IAccessor<T> _accessor;

        Aggregator<T>[] _totalAggregators;
        string[] _totalSelectors;

        string[] _groupSummaryTypes;
        string[] _groupSelectors;
        Stack<Aggregator<T>[]> _groupAggregatorsStack;

        static AggregatorRegistry<T> _aggregatorRegistry = new AggregatorRegistry<T>();

        static AggregateCalculator() {
            RegisterDefaultAggregators(_aggregatorRegistry);
        }

        internal AggregateCalculator(IEnumerable data, IAccessor<T> accessor, IEnumerable<SummaryInfo> totalSummary, IEnumerable<SummaryInfo> groupSummary) {
            _data = data;
            _accessor = accessor;

            if (totalSummary != null) {
                _totalAggregators = totalSummary.Select(i => CreateAggregator(i.SummaryType)).ToArray();
                _totalSelectors = totalSummary.Select(i => i.Selector).ToArray();
            }

            if (groupSummary != null) {
                _groupSummaryTypes = groupSummary.Select(i => i.SummaryType).ToArray();
                _groupSelectors = groupSummary.Select(i => i.Selector).ToArray();
                _groupAggregatorsStack = new Stack<Aggregator<T>[]>();
            }
        }

        internal object[] Run() {
            foreach(var item in _data)
                ProcessItem(item);

            if(_totalAggregators != null)
                return Finish(_totalAggregators);

            return null;
        }

        public static void RegisterAggregator<TAggregator>(string summaryType) where TAggregator : Aggregator<T> {
            _aggregatorRegistry.RegisterAggregator<TAggregator>(summaryType);
        }

        public static void RegisterAggregator(string summaryType, Func<IAccessor<T>, Aggregator<T>> factory) {
            _aggregatorRegistry.RegisterAggregator(summaryType, factory);
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
            var aggregator = _aggregatorRegistry.CreateAggregator(summaryType, _accessor);

            if (aggregator == null)
                throw new NotSupportedException();

            return aggregator;
        }

        static void RegisterDefaultAggregators(AggregatorRegistry<T> aggregatorRegistry) {
            aggregatorRegistry.RegisterAggregator<SumAggregator<T>>(AggregateName.SUM);
            aggregatorRegistry.RegisterAggregator<MinAggregator<T>>(AggregateName.MIN);
            aggregatorRegistry.RegisterAggregator<MaxAggregator<T>>(AggregateName.MAX);
            aggregatorRegistry.RegisterAggregator<AvgAggregator<T>>(AggregateName.AVG);
            aggregatorRegistry.RegisterAggregator(AggregateName.COUNT, accessor => new CountAggregator<T>(accessor, false));

            aggregatorRegistry.RegisterAggregator<RemoteCountAggregator<T>>(AggregateName.REMOTE_COUNT);
            aggregatorRegistry.RegisterAggregator<RemoteAvgAggregator<T>>(AggregateName.REMOTE_AVG);
        }
    }
}
