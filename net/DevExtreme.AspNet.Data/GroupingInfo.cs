using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    public class GroupingInfo : SortingInfo {
        public string GroupInterval;
        public bool? IsExpanded;

        public bool GetIsExpanded() {
            if(!IsExpanded.HasValue)
                return true;

            return IsExpanded.Value;
        }
    }

}
