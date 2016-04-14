using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.RemoteGrouping {

    class RemoteGroupAccessor : IAccessor<IRemoteGroup> {
        public object Read(IRemoteGroup container, string selector) {
            if(selector.StartsWith("K"))
                return container.GetKey(int.Parse(selector.Substring(1)));

            if(selector.StartsWith("T"))
                return container.GetTotalAggregate(int.Parse(selector.Substring(1)));

            if(selector.StartsWith("G"))
                return container.GetGroupAggregate(int.Parse(selector.Substring(1)));

            throw new ArgumentException();
        }
    }

}
