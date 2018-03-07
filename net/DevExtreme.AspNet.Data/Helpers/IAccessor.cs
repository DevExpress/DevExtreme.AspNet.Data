using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Helpers {

    public interface IAccessor<T> {
        object Read(T container, string selector);
    }

}
