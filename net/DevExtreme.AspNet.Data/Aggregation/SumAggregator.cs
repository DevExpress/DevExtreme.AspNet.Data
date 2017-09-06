using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class SumAggregator<T> : Aggregator<T> {
        object _sum;
        Type _type;

        public SumAggregator(IAccessor<T> accessor)
            : base(accessor) {
        }

        public override void Step(T container, string selector) {
            var value = Accessor.Read(container, selector);

            if(value != null) {
                if(_type == null) {
                    _type = (value is Double || value is Single) ? typeof(Double) : typeof(Decimal);
                    _sum = Activator.CreateInstance(_type);
                }

                try {
                    value = Convert.ChangeType(value, _type, CultureInfo.InvariantCulture);

                    if(_type == typeof(Double)) {
                        _sum = (Double)_sum + (Double)value;
                    } else {
                        _sum = (Decimal)_sum + (Decimal)value;
                    }

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
