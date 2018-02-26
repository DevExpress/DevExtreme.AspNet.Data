using System;
using System.Collections.Generic;

namespace DevExtreme.AspNet.Data.Aggregation {
    class AggregatorRegistry<T> {
        private Dictionary<string, Func<IAccessor<T>, Aggregator<T>>> aggregatorFactories
            = new Dictionary<string, Func<IAccessor<T>, Aggregator<T>>>();

        public void RegisterAggregator<TAggregator>(string summaryType) where TAggregator : Aggregator<T> {
            aggregatorFactories[summaryType] = CreateAggregator<TAggregator>;
        }

        public void RegisterAggregator(string summaryType, Func<IAccessor<T>, Aggregator<T>> factory) {
            aggregatorFactories[summaryType] = factory;
        }

        public Aggregator<T> CreateAggregator(string summaryType, IAccessor<T> accessor) {
            if (aggregatorFactories.TryGetValue(summaryType, out var factory)) {
                return factory(accessor);
            }

            return null;
        }

        private static Aggregator<T> CreateAggregator<TAggregator>(IAccessor<T> accessor) where TAggregator : Aggregator<T> {
            var aggregatorType = typeof(TAggregator);
            return Activator.CreateInstance(aggregatorType, accessor) as Aggregator<T>;
        }
    }
}
