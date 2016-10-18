using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    class DataSourceLoadResult {
        public IEnumerable data;

        public int totalCount = -1;

        public int groupCount = -1;

        public object[] summary;

        internal bool IsDataOnly() {
            return totalCount == -1 && summary == null && groupCount == -1;
        }
    }

}
