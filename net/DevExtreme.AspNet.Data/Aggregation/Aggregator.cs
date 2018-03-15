using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    /// <summary>
    /// Represents a data aggregator.
    /// </summary>
    /// <typeparam name="T">The type of the data items to be aggregated.</typeparam>
    public abstract class Aggregator<T> {
        /// <summary>
        /// Provides the means to read data item properties.
        /// </summary>
        protected readonly IAccessor<T> Accessor;

        /// <summary>
        /// Initializes a new instance of the Aggregator class with an object that provides the means to read data item properties.
        /// </summary>
        /// <param name="accessor">An object that provides the means to read data item properties.</param>
        protected Aggregator(IAccessor<T> accessor) {
            Accessor = accessor;
        }

        /// <summary>
        /// A callback invoked once for each data item.
        /// </summary>
        /// <param name="container">A data item.</param>
        /// <param name="selector">The name or path to the property whose value should be aggregated.</param>
        public abstract void Step(T container, string selector);

        /// <summary>
        /// A callback invoked at the final stage of the aggregation.
        /// </summary>
        /// <returns>The result of the aggregation.</returns>
        public abstract object Finish();
    }

}
