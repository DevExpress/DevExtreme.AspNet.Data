using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    /// <summary>
    /// Represents a sorting parameter.
    /// </summary>
    public class SortingInfo {
        /// <summary>
        /// The data field to be used for sorting.
        /// </summary>
        public string Selector { get; set; }

        /// <summary>
        /// A flag indicating whether data should be sorted in a descending order.
        /// </summary>
        public bool Desc { get; set; }
    }

}
