using DevExtreme.AspNet.Data.Aggregation;
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

        public override void Step(T dataitem, string selector) {
            var group = dataitem as IRemoteGroup;
            _count += group.Count;
        }

        public override object Finish() {
            return _count;
        }
    }

}
