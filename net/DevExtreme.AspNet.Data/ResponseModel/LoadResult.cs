using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.ResponseModel {

    /// <summary>
    /// Represents a load result.
    /// </summary>
    public class LoadResult {
        /// <summary>
        /// A resulting dataset.
        /// </summary>
        public IEnumerable data { get; set; }

        /// <summary>
        /// The total number of data objects in the resulting dataset.
        /// </summary>
        [DefaultValue(-1), JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int totalCount { get; set; } = -1;

        /// <summary>
        /// The number of top-level groups in the resulting dataset.
        /// </summary>
        [DefaultValue(-1), JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int groupCount { get; set; } = -1;

        /// <summary>
        /// Total summary calculation results.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object[] summary { get; set; }
    }

}
