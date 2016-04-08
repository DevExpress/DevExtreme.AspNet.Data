using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class AvgAggregator : Aggregator {
        CountAggregator _counter = new CountAggregator();
        SumAggregator _summator = new SumAggregator();

        public override void Step(object value) {
            _counter.Step(value);
            _summator.Step(value);
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
