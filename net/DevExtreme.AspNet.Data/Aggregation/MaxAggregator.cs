using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class MaxAggregator : Aggregator {
        object _max = UNINITIALIZED;

        public override void Step(object value) {
            if(IsNotInitialized(_max) || Comparer<object>.Default.Compare(value, _max) > 0)
                _max = value;
        }

        public override object Finish() {
            if(IsNotInitialized(_max))
                return null;

            return _max;
        }
    }

}
