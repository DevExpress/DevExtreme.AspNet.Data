using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Helpers {

    /// <summary>
    /// Provides the means to access object properties.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    public interface IAccessor<T> {
        /// <summary>
        /// Gets the value of an object property.
        /// </summary>
        /// <param name="container">An object with the property to access.</param>
        /// <param name="selector">The name or path to the property.</param>
        /// <returns>The obtained value.</returns>
        object Read(T container, string selector);
    }

}
