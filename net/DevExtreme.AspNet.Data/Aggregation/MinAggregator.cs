using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class MinAggregator<T> : Aggregator<T> {
        object _min = null;

        public MinAggregator(IAccessor<T> accessor)
            : base(accessor) {
        }

        public override void Step(T container, string selector) {
            var value = Accessor.Read(container, selector);

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
