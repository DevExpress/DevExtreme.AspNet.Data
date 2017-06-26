using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevExtreme.AspNet.Data {

    static class Accessors {
        public static readonly IAccessor<IDictionary<string, object>> Dict = new DictImpl();
        public static readonly IAccessor<AnonType> AnonType = new AnonTypeImpl();

        class DictImpl : IAccessor<IDictionary<string, object>> {
            public object Read(IDictionary<string, object> container, string selector) {
                return container[selector];
            }
        }

        class AnonTypeImpl : IAccessor<AnonType> {
            public object Read(AnonType container, string selector) {
                return container[int.Parse(selector.Substring(1))];
            }
        }
    }

}
