using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.RemoteGrouping {

    class RemoteAvgAggregator<T> : Aggregator<T> {
        Aggregator<T> _countAggregator;
        SumAggregator<T> _valueAggregator;

        public RemoteAvgAggregator(IAccessor<T> accessor)
            : base(accessor) {
            _countAggregator = new SumAggregator<T>(accessor);
            _valueAggregator = new SumAggregator<T>(accessor);
        }

        public override void Step(T container, string selector) {
            _countAggregator.Step(container, AnonType.IndexToField(1 + AnonType.FieldToIndex(selector)));
            _valueAggregator.Step(container, selector);
        }

        public override object Finish() {
            var count = Convert.ToInt32(_countAggregator.Finish());
            if(count == 0)
                return null;

            var valueAccumulator = _valueAggregator.GetAccumulator();
            valueAccumulator.Divide(count);
            return valueAccumulator.GetValue();
        }
    }

}
