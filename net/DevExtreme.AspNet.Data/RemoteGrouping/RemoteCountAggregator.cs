using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.RemoteGrouping {

    class RemoteCountAggregator<T> : Aggregator<T> {
        int _count = 0;

        public RemoteCountAggregator(IAccessor<T> accessor)
            : base(accessor) {
        }

        public override void Step(T tuple, string _) {
            _count += (int)TupleUtils.ReadItem(tuple, 0);
        }

        public override object Finish() {
            return _count;
        }
    }

}
