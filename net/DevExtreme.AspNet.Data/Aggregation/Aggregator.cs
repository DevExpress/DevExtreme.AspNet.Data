using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    abstract class Aggregator {
        public abstract void Step(object value);
        public abstract object Finish();
    }

}
