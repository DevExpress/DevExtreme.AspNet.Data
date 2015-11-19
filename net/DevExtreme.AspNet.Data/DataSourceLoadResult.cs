using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if MVC5
using System.Web.Mvc;
#else
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
#endif

namespace DevExtreme.AspNet.Data {

    public static class DataSourceLoadResult {
        public static ContentResult Create<T>(IEnumerable<T> source, DataSourceLoadOptions loadOptions) {
            return new DataSourceLoadResult<T>(source, loadOptions);
        }
    }

    public class DataSourceLoadResult<T> : ContentResult {

        public DataSourceLoadResult(IEnumerable<T> source, DataSourceLoadOptions loadOptions) {

#if MVC5
            ContentType = "application/json";
#else
            ContentType = new MediaTypeHeaderValue("application/json");
#endif

            Content = JsonConvert.SerializeObject(new DataSourceLoader().Load(source, loadOptions));
        }

    }
}
