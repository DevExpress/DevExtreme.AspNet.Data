using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExtreme.AspNet.Data.Tests {

    #region class
    class TotalSalesAggregator<T> : Aggregator<T> {
        decimal _total = 0;

        public TotalSalesAggregator(IAccessor<T> accessor)
            : base(accessor) {
        }

        public override void Step(T container, string selector) {
            var quantity = Convert.ToInt32(Accessor.Read(container, "Quantity"));
            var unitPrice = Convert.ToDecimal(Accessor.Read(container, "UnitPrice"));
            var discount = Convert.ToDecimal(Accessor.Read(container, "Discount"));
            _total += quantity * unitPrice * (1 - discount);
        }

        public override object Finish() {
            return _total;
        }
    }
    #endregion
}
