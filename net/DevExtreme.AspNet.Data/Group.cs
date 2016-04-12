using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    class Group {
        public object key;
        public IList<object> items;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? count;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object[] summary;
    }

}
