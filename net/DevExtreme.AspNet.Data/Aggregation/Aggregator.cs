using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Aggregation {

    /// <summary>
    /// An abstract class that represents a data aggregator.
    /// </summary>
    /// <typeparam name="T">The type of the data items to be aggregated.</typeparam>
    public abstract class Aggregator<T> {
        /// <summary>
        /// Provides the means to access data item properties.
        /// </summary>
        protected readonly IAccessor<T> Accessor;

        /// <summary>
        /// Initializes a new instance of the Aggregator class with an object that provides the means to access data item properties.
        /// </summary>
        /// <param name="accessor">An object that provides the means to access data item properties.</param>
        protected Aggregator(IAccessor<T> accessor) {
            Accessor = accessor;
        }

        /// <summary>
        /// A method that is called once for each data item in the collection.
        /// </summary>
        /// <param name="container">A data item.</param>
        /// <param name="selector">The name of the property whose value should be processed.</param>
        public abstract void Step(T container, string selector);

        /// <summary>
        /// A method that is called at the final stage of the calculation.
        /// </summary>
        /// <returns>The result of the calculation.</returns>
        public abstract object Finish();
    }

}
