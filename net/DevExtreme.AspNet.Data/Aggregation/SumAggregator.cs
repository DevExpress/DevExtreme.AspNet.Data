using DevExtreme.AspNet.Data.Aggregation.Accumulators;
using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    class SumAggregator<T> : Aggregator<T> {
        IAccumulator _accumulator;

        public SumAggregator(IAccessor<T> accessor)
            : base(accessor) {
        }

        public override void Step(T container, string selector) {
            var value = Accessor.Read(container, selector);

            if(value != null) {
                if(_accumulator == null)
                    _accumulator = AccumulatorFactory.Create(value.GetType());

                try {
                    _accumulator.Add(value);
                } catch(FormatException) {
                } catch(InvalidCastException) {
                }
            }
        }

        public override object Finish() {
            return _accumulator?.GetValue();
        }

        public IAccumulator GetAccumulator() {
            return _accumulator;
        }

    }

}
