using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class CountAggregator : Aggregator {
        int _count;
        bool _skipNulls;

        public CountAggregator(bool skipNulls) {
            _skipNulls = skipNulls;
        }

        public override void Step(object value) {
            if(!_skipNulls || value != null)
                _count++;
        }

        public override object Finish() {
            return _count;
        }

    }

}
