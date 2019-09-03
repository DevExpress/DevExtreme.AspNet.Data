using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    /// <summary>
    /// Provides static methods for loading data from collections that implement the
    /// <see cref="System.Collections.Generic.IEnumerable{T}"/> or <see cref="System.Linq.IQueryable{T}"/> interface.
    /// </summary>
    public class DataSourceLoader {

        /// <summary>
        /// Loads data from a collection that implements the <see cref="System.Collections.Generic.IEnumerable{T}"/> interface.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection.</typeparam>
        /// <param name="source">A collection that implements the <see cref="System.Collections.Generic.IEnumerable{T}"/> interface.</param>
        /// <param name="options">Data processing settings when loading data.</param>
        /// <returns>The load result.</returns>
        public static LoadResult Load<T>(IEnumerable<T> source, DataSourceLoadOptionsBase options) {
            return Load(source.AsQueryable(), options);
        }

        /// <summary>
        /// Loads data from a collection that implements the <see cref="System.Linq.IQueryable{T}"/> interface.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection.</typeparam>
        /// <param name="source">A collection that implements the <see cref="System.Linq.IQueryable{T}"/> interface.</param>
        /// <param name="options">Data processing settings when loading data.</param>
        /// <returns>The load result.</returns>
        public static LoadResult Load<T>(IQueryable<T> source, DataSourceLoadOptionsBase options) {
            return LoadAsync(source, options, CancellationToken.None, true).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously loads data from a collection that implements the <see cref="System.Linq.IQueryable{T}"/> interface.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection.</typeparam>
        /// <param name="source">A collection that implements the <see cref="System.Linq.IQueryable{T}"/> interface.</param>
        /// <param name="options">Data processing settings when loading data.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken"/> object that delivers a cancellation notice to the running operation.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task{TResult}"/> object that represents the asynchronous operation.
        /// The task result contains the load result.
        /// </returns>
        public static Task<LoadResult> LoadAsync<T>(IQueryable<T> source, DataSourceLoadOptionsBase options, CancellationToken cancellationToken = default(CancellationToken)) {
            return LoadAsync(source, options, cancellationToken, false);
        }

        static Task<LoadResult> LoadAsync<T>(IQueryable<T> source, DataSourceLoadOptionsBase options, CancellationToken ct, bool sync) {
            return new DataSourceLoaderImpl<T>(source, options, ct, sync).LoadAsync();
        }

    }

}
