using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class CountAggregator : Aggregator {
        int _count;

        public override void Step(object value) {
            _count++;
        }

        public override object Finish() {
            return _count;
        }

    }

}
