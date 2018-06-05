using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Aggregation {

    class RemoteAvgAggregator<T> : Aggregator<T> {
        Aggregator<T> _countAggregator;
        Aggregator<T> _valueAggregator;

        public RemoteAvgAggregator(IAccessor<T> accessor)
            : base(accessor) {
            _countAggregator = new SumAggregator<T>(accessor);
            _valueAggregator = new SumAggregator<T>(accessor);
        }

        public override void Step(T tuple, string itemIndex) {
            _countAggregator.Step(tuple, Convert.ToString(1 + Int32.Parse(itemIndex)));
            _valueAggregator.Step(tuple, itemIndex);
        }

        public override object Finish() {
            return (decimal?)_valueAggregator.Finish() / (decimal?)_countAggregator.Finish();
        }
    }

}
