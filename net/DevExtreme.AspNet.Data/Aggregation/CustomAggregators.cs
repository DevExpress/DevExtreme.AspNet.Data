using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;

namespace DevExtreme.AspNet.Data.Aggregation {
    public static class CustomAggregators {
        private static readonly Dictionary<string, Type> _aggregatorTypes = new Dictionary<string, Type>();

        public static void RegisterAggregator(string summaryType, Type aggregatorType) {
            _aggregatorTypes[summaryType] = aggregatorType;
        }

        internal static Aggregator<T> CreateAggregator<T>(string summaryType, IAccessor<T> accessor) {
            if (_aggregatorTypes.TryGetValue(summaryType, out var aggregatorType)) {
                var genericAggregatorType = aggregatorType.MakeGenericType(typeof(T));
                return (Aggregator<T>)Activator.CreateInstance(genericAggregatorType, accessor);
            }

            return null;
        }

#if DEBUG
        internal static void Clear() {
            _aggregatorTypes.Clear();
        }
#endif
    }
}
