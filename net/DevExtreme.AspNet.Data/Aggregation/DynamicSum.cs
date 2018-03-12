using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExtreme.AspNet.Data.Aggregation {

    static class DynamicSum {

        public static object Calculate<T>(IEnumerable<T> source, Func<T, object> selector) {
            var summator = new SumAggregator<object>(new IdentityAccessor());
            foreach(var i in source.Select(selector))
                summator.Step(i, null);
            return summator.Finish();
        }

        class IdentityAccessor : IAccessor<object> {
            public object Read(object container, string selector) {
                return container;
            }
        }

    }

}
