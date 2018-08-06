using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.ResponseModel {

    /// <summary>
    /// Represents a group in the resulting dataset.
    /// </summary>
    public class Group {
        /// <summary>
        /// The group's key.
        /// </summary>
        public object key { get; set; }

        /// <summary>
        /// Subgroups or data objects.
        /// </summary>
        public IList items { get; set; }

        /// <summary>
        /// The count of items in the group.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? count { get; set; }

        /// <summary>
        /// Group summary calculation results.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object[] summary { get; set; }
    }

}
