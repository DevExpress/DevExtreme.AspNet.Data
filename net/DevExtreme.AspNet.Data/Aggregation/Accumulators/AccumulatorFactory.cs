using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Aggregation.Accumulators {

    static class AccumulatorFactory {

        public static IAccumulator Create(Type type) {
            if(type == typeof(Double) || type == typeof(Single))
                return new DoubleAccumulator();

            if(type == typeof(TimeSpan))
                return new TimeSpanAccumulator();

            return new DecimalAccumulator();
        }

    }

}
