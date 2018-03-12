using DevExtreme.AspNet.Data.Aggregation.Accumulators;
using DevExtreme.AspNet.Data.Helpers;
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

            var accumulator = _summator.GetAccumulator();
            accumulator.Divide(count);
            return accumulator.GetValue();
        }
    }

}
