using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.ResponseModel {

    public class LoadResult {
        public IEnumerable data;

        [DefaultValue(-1), JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int totalCount = -1;

        [DefaultValue(-1), JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int groupCount = -1;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object[] summary;
    }

}
