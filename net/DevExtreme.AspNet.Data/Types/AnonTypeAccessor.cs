using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Types {

    class AnonTypeAccessor : IAccessor<AnonType> {
        public static readonly AnonTypeAccessor Instance = new AnonTypeAccessor();

        private AnonTypeAccessor() {
        }

        public object Read(AnonType container, string selector) {
            return container[AnonType.FieldToIndex(selector)];
        }
    }

}
