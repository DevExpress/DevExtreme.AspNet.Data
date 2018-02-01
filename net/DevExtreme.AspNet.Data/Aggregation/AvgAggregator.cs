using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class AvgAggregator<T> : Aggregator<T> {
        Aggregator<T> _counter;
        SumAggregator<T> _summator;

        public AvgAggregator(IAccessor<T> accessor)
            : base(accessor) {
            _counter = new CountAggregator<T>(accessor, true);
            _summator = new SumAggregator<T>(accessor);
        }

        public override void Step(T container, string selector) {
            _counter.Step(container, selector);
            _summator.Step(container, selector);
        }

        public override object Finish() {
            var count = (int)_counter.Finish();
            if(count == 0)
                return null;

            var sum = _summator.Finish();

            if(sum is Double doubleSum)
                return doubleSum / count;

            return (decimal)sum / count;
        }
    }

}
