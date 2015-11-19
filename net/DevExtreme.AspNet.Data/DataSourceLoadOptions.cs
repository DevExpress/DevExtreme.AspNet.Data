using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace DevExtreme.AspNet.Data {

#if MVC5
    using System.Web.Mvc;
    [ModelBinder(typeof(DataSourceLoadOptionsBinder))]
#else
    using Microsoft.AspNet.Mvc;
    [ModelBinder(BinderType = typeof(DataSourceLoadOptionsBinder))]
#endif
    public class DataSourceLoadOptions {
        public bool RequireTotalCount { get; set; }
        public bool IsCountQuery { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public SortingInfo[] Sort { get; set; }
        public IList Filter { get; set; }
    }

}
