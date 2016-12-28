using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Types {

    class AnonTypeAccessor : IAccessor<AnonType> {

        public object Read(AnonType container, string selector) {
            if(selector.StartsWith(AnonType.ITEM_PREFIX))
                return container[int.Parse(selector.Substring(1))];
            throw new ArgumentException();
        }

    }

}
