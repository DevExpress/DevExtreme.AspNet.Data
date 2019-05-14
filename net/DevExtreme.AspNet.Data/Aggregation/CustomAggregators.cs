using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;

namespace DevExtreme.AspNet.Data.Aggregation {
    /// <summary>
    /// Provides methods that manipulate custom aggregators.
    /// </summary>
    public static class CustomAggregators {
        private static readonly Dictionary<string, Type> _aggregatorTypes = new Dictionary<string, Type>();

        /// <summary>
        /// Registers a custom aggregator.
        /// </summary>
        /// <param name="summaryType">The aggregator's string identifier.</param>
        /// <param name="aggregatorType">The aggregator's type declaration without the generic type parameter T.</param>
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

        internal static bool IsRegistered(string summaryType) {
            return _aggregatorTypes.ContainsKey(summaryType);
        }

#if DEBUG
        internal static void Clear() {
            _aggregatorTypes.Clear();
        }
#endif
    }
}
