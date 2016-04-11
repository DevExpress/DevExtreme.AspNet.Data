using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class MaxAggregator : Aggregator {
        object _max = null;

        public override void Step(object value) {
            if(value is IComparable) {
                if(_max == null || Comparer<object>.Default.Compare(value, _max) > 0)
                    _max = value;
            }
        }

        public override object Finish() {
            return _max;
        }
    }

}
