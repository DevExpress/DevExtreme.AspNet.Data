using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Helpers {

    /// <summary>
    /// Defines a method that a class implements to access data item properties.
    /// </summary>
    /// <typeparam name="T">The type of the data item.</typeparam>
    public interface IAccessor<T> {
        /// <summary>
        /// Gets the value of a data item property.
        /// </summary>
        /// <param name="container">A data item.</param>
        /// <param name="selector">The property's name.</param>
        /// <returns>The property's value.</returns>
        object Read(T container, string selector);
    }

}
