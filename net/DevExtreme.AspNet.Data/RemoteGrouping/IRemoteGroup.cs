using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.RemoteGrouping {

    interface IRemoteGroup {
        int Count { get; }
        object GetKey(int index);
        object GetTotalAggregate(int index);
        object GetGroupAggregate(int index);
    }

}
