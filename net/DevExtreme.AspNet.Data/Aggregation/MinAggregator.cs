using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class MinAggregator : Aggregator {
        object _min = null;

        public override void Step(object value) {
            if(value is IComparable) {
                if(_min == null || Comparer<object>.Default.Compare(value, _min) < 0)
                    _min = value;
            }
        }

        public override object Finish() {
            return _min;
        }

    }

}
