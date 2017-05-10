using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class AvgAggregator<T> : Aggregator<T> {
        Aggregator<T> _counter;
        SumAggregator<T> _summator;

        public AvgAggregator(IAccessor<T> accessor, Aggregator<T> counter)
            : base(accessor) {
            _counter = counter;
            _summator = new SumAggregator<T>(accessor);
        }

        public override void Step(T container, string selector) {
            _counter.Step(container, selector);
            _summator.Step(container, selector);
        }

        public override object Finish() {
            var count = _counter.Finish();
            var sum = _summator.Finish();

            if(Equals(count, 0))
                return null;

            return (decimal)sum / (int)count;
        }
    }

}
