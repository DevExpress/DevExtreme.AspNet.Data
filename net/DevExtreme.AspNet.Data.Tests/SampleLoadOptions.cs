using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Tests {

    class SampleLoadOptions : DataSourceLoadOptionsBase {
        public List<string> ExpressionLog = new List<string>();

        public SampleLoadOptions() {
            UseEnumerableOnce = true;
            ExpressionWatcher = x => ExpressionLog.Add(x.ToString());
        }

    }

}
