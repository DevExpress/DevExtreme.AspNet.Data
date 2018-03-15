using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Helpers {

    /// <summary>
    /// Provides the means to read object properties.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    public interface IAccessor<T> {
        /// <summary>
        /// Reads an object property.
        /// </summary>
        /// <param name="container">An object with the property to read.</param>
        /// <param name="selector">The name or path to the property.</param>
        /// <returns>The property's value.</returns>
        object Read(T container, string selector);
    }

}
