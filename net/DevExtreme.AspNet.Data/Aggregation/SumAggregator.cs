using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class SumAggregator<T> : Aggregator<T> {
        object _sum;

        public SumAggregator(IAccessor<T> accessor)
            : base(accessor) {
        }

        public override void Step(T container, string selector) {
            var value = Accessor.Read(container, selector);

            if(value != null) {
                if(_sum == null) {
                    if(value is Double || value is Single)
                        _sum = 0d;
                    else
                        _sum = 0m;
                }

                try {
                    if(_sum is Double)
                        _sum = (Double)_sum + Convert.ToDouble(value, CultureInfo.InvariantCulture);
                    else
                        _sum = (Decimal)_sum + Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                } catch(FormatException) {
                } catch(InvalidCastException) {
                }
            }
        }

        public override object Finish() {
            return _sum;
        }

    }

}
