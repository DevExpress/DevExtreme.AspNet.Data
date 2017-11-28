using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExtreme.AspNet.Data {

    public class DataSourceLoader {

        /// <summary>
        /// Sample description
        /// </summary>
        /// <typeparam name="T">Sample description</typeparam>
        /// <param name="source">Sample description</param>
        /// <param name="options">Sample description</param>
        /// <returns>Sample description</returns>
        public static LoadResult Load<T>(IEnumerable<T> source, DataSourceLoadOptionsBase options) {
            return Load(source.AsQueryable(), options);
        }

        public static LoadResult Load<T>(IQueryable<T> source, DataSourceLoadOptionsBase options) {
            return new DataSourceLoaderImpl<T>(source, options).Load();
        }

    }

}
