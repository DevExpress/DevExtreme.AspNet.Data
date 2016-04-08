using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class MinAggregator : Aggregator {
        object _min = UNINITIALIZED;

        public override void Step(object value) {
            if(IsNotInitialized(_min) || Comparer<object>.Default.Compare(value, _min) < 0)
                _min = value;
        }

        public override object Finish() {
            if(IsNotInitialized(_min))
                return null;

            return _min;
        }

    }

}
