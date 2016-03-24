using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    public abstract class DataSourceLoadOptionsBase {
        public bool RequireTotalCount { get; set; }
        public bool IsCountQuery { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public SortingInfo[] Sort { get; set; }
        public IList Filter { get; set; }
    }

}
