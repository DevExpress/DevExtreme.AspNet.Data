using System;
using System.Globalization;
using System.Linq;

namespace DevExtreme.AspNet.Data.Aggregation.Accumulators {

    class DoubleAccumulator : IAccumulator {
        double _value;

        public void Add(object value) {
            _value += Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        public void Divide(int divider) {
            _value /= divider;
        }

        public object GetValue() {
            return _value;
        }
    }

}
