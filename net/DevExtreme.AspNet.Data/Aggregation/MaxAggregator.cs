using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class MaxAggregator<T> : Aggregator<T> {
        object _max = null;

        public MaxAggregator(IAccessor<T> accessor)
            : base(accessor) {
        }

        public override void Step(T container, string selector) {
            var value = Accessor.Read(container, selector);

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
