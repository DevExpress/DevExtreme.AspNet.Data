using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExtreme.AspNet.Data.Helpers {

    static class Accessors {
        public static readonly IAccessor<AnonType> AnonType = new AnonTypeImpl();

        class AnonTypeImpl : IAccessor<AnonType> {
            public object Read(AnonType container, string selector) {
                return container[int.Parse(selector.Substring(1))];
            }
        }
    }

}
