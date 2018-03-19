using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Helpers {

    /// <summary>
    /// Allows reading object properties.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    public interface IAccessor<T> {
        /// <summary>
        /// Reads an object property.
        /// </summary>
        /// <param name="container">An object whose property should be read.</param>
        /// <param name="selector">The name or path to the property.</param>
        /// <returns>The property's value.</returns>
        object Read(T container, string selector);
    }

}
