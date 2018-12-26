using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExtreme.AspNet.Data {

    /// <summary>
    /// Provides static methods for loading data from collections that implement the IEnumerable&lt;T&gt; or IQueryable&lt;T&gt; interface.
    /// </summary>
    public class DataSourceLoader {

        /// <summary>
        /// Loads data from a collection that implements the IEnumerable&lt;T&gt; interface.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection.</typeparam>
        /// <param name="source">A collection that implements the IEnumerable&lt;T&gt; interface.</param>
        /// <param name="options">Data processing settings when loading data.</param>
        /// <returns>The load result.</returns>
        public static LoadResult Load<T>(IEnumerable<T> source, DataSourceLoadOptionsBase options) {
            return Load(source.AsQueryable(), options);
        }

        /// <summary>
        /// Loads data from a collection that implements the IQueryable&lt;T&gt; interface.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection.</typeparam>
        /// <param name="source">A collection that implements the IQueryable&lt;T&gt; interface.</param>
        /// <param name="options">Data processing settings when loading data.</param>
        /// <returns>The load result.</returns>
        public static LoadResult Load<T>(IQueryable<T> source, DataSourceLoadOptionsBase options) {
            return new DataSourceLoaderImpl<T>(source, options).Load();
        }

    }

}
