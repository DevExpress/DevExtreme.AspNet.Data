using System.Collections.Generic;

namespace DevExtreme.AspNet.Data.Tests {

    public class SampleLoadOptions : DataSourceLoadOptionsBase {
        public List<string> ExpressionLog = new List<string>();

        public SampleLoadOptions() {
            UseEnumerableOnce = true;
            ExpressionWatcher = x => ExpressionLog.Add(x.ToString());
        }

    }

}
