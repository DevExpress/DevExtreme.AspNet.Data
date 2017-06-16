using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevExtreme.AspNet.Data {

    class DictAccessor : IAccessor<IDictionary<string, object>> {

        public object Read(IDictionary<string, object> container, string selector) {
            return container[selector];
        }

    }

}
