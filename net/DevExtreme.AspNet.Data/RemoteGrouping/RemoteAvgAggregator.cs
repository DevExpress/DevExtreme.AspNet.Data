using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Data.Types;
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

        public override void Step(T container, string selector) {
            var itemIndex = Int32.Parse(selector);

            _countAggregator.Step(container, AnonType.ITEM_PREFIX + (itemIndex + 1));
            _valueAggregator.Step(container, AnonType.ITEM_PREFIX + itemIndex);
        }

        public override object Finish() {
            var count = (decimal)_countAggregator.Finish();
            var value = _valueAggregator.Finish();

            if(count == 0m)
                return null;

            return (decimal)value / count;
        }
    }

}
