using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    abstract class Aggregator {
        protected static readonly object UNINITIALIZED = new object();

        protected bool IsNotInitialized(object value) {
            return ReferenceEquals(value, UNINITIALIZED);
        }

        public abstract void Step(object value);
        public abstract object Finish();
    }

}
