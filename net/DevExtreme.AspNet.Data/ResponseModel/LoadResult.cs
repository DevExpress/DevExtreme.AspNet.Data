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
        /// A result dataset.
        /// </summary>
        public IEnumerable data;

        /// <summary>
        /// The total count of data objects in the result dataset.
        /// </summary>
        [DefaultValue(-1), JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int totalCount = -1;

        /// <summary>
        /// The count of top-level groups in the result dataset.
        /// </summary>
        [DefaultValue(-1), JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int groupCount = -1;

        /// <summary>
        /// Total summary calculation results.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object[] summary;
    }

}
