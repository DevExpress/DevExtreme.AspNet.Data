using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    class DataSourceLoadResult {
        public IEnumerable data;

        [DefaultValue(-1), JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int totalCount = -1;

        [DefaultValue(-1), JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int groupCount = -1;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object[] summary;

        internal bool IsDataOnly() {
            return totalCount == -1 && summary == null && groupCount == -1;
        }
    }

}
