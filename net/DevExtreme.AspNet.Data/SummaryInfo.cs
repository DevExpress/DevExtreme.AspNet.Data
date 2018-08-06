using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    /// <summary>
    /// Represents a group or total summary definition.
    /// </summary>
    public class SummaryInfo {
        /// <summary>
        /// The data field to be used for calculating the summary.
        /// </summary>
        public string Selector { get; set; }

        /// <summary>
        /// An aggregate function: "sum", "min", "max", "avg", or "count".
        /// </summary>
        public string SummaryType { get; set; }
    }

}
