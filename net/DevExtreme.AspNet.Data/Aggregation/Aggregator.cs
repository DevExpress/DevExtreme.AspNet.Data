using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    public abstract class Aggregator<T> {
        protected readonly IAccessor<T> Accessor;

        protected Aggregator(IAccessor<T> accessor) {
            Accessor = accessor;
        }

        public abstract void Step(T container, string selector);
        public abstract object Finish();
    }

}
