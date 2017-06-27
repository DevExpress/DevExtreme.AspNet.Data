using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.ResponseModel {

    public class Group {
        public object key;
        public IList items;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? count;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object[] summary;
    }

}
