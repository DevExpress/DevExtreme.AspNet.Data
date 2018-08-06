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
            _countAggregator.Step(container, AnonType.IndexToField(1 + AnonType.FieldToIndex(selector)));
            _valueAggregator.Step(container, selector);
        }

        public override object Finish() {
            var count = (decimal?)_countAggregator.Finish();
            if(count == 0)
                return null;

            return (decimal?)_valueAggregator.Finish() / count;
        }
    }

}
