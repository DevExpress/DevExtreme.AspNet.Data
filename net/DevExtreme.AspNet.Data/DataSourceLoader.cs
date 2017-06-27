using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExtreme.AspNet.Data {

    public class DataSourceLoader {

        public static LoadResult Load<T>(IEnumerable<T> source, DataSourceLoadOptionsBase options) {
            return Load(source.AsQueryable(), options);
        }

        public static LoadResult Load<T>(IQueryable<T> source, DataSourceLoadOptionsBase options) {
            return new DataSourceLoaderImpl<T>(source, options).Load();
        }

    }

}
