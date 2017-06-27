using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExtreme.AspNet.Data {

    public class DataSourceLoader {

        public static DataSourceLoadResult Load<T>(IEnumerable<T> source, DataSourceLoadOptionsBase options) {
            return Load(source.AsQueryable(), options);
        }

        public static DataSourceLoadResult Load<T>(IQueryable<T> source, DataSourceLoadOptionsBase options) {
            return new DataSourceLoaderImpl<T>(source, options).Load();
        }

    }

}
