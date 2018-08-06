using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    /// <summary>
    /// Represents a grouping level to be applied to data.
    /// </summary>
    public class GroupingInfo : SortingInfo {
        /// <summary>
        /// A value that groups data in ranges of a given length or date/time period.
        /// </summary>
        public string GroupInterval { get; set; }

        /// <summary>
        /// A flag indicating whether the group's data objects should be returned.
        /// </summary>
        public bool? IsExpanded { get; set; }

        /// <summary>
        /// Returns the value of the IsExpanded field or <c>true</c> if this value is <c>null</c>.
        /// </summary>
        /// <returns>The value of the IsExpanded field or <c>true</c> if this value is <c>null</c>.</returns>
        public bool GetIsExpanded() {
            if(!IsExpanded.HasValue)
                return true;

            return IsExpanded.Value;
        }
    }

}
