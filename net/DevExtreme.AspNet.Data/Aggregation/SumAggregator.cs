using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class SumAggregator : Aggregator {
        decimal? _sum;

        public override void Step(object value) {
            if(value != null) {
                if(!_sum.HasValue)
                    _sum = 0;

                try {
                    _sum += Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                } catch(FormatException) {
                }
            }
        }

        public override object Finish() {
            return _sum;
        }

    }

}
