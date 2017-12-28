using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.ResponseModel {

    /// <summary>
    /// Represents a group in the result dataset.
    /// </summary>
    public class Group {
        /// <summary>
        /// The group's key.
        /// </summary>
        public object key;

        /// <summary>
        /// Subgroups or data objects.
        /// </summary>
        public IList items;

        /// <summary>
        /// The count of items in the group.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? count;

        /// <summary>
        /// Group summary calculation results.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object[] summary;
    }

}
